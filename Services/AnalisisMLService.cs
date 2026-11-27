using Microsoft.ML;
using Microsoft.ML.Data;
using SmartProcessAI.Models;

namespace SmartProcessAI.Services;

// una fila del dataset de entrenamiento
public class TareaInput
{
    [LoadColumn(0)] public float TiempoMinutos { get; set; }
    [LoadColumn(1)] public float FrecuenciaMensual { get; set; }
    [LoadColumn(2)] public float NumeroPasos { get; set; }
    [LoadColumn(3)] public float CantidadHerramientas { get; set; }
    [LoadColumn(4)] public bool EsAutomatizable { get; set; }
}

// resultado del modelo de clasificación
public class PrediccionTarea
{
    [ColumnName("PredictedLabel")] public bool EsAutomatizable { get; set; }
    [ColumnName("Probability")] public float Probabilidad { get; set; }
    public float Score { get; set; }
}

// resultado de la detección de tiempos atípicos
public class TareaAtipica
{
    public int TareaId { get; set; }
    public float TiempoMinutos { get; set; }
    public bool EsAtipica { get; set; }
    public float Puntaje { get; set; }
}

// puntaje del modelo de anomalías (uso interno)
public class PuntajeAnomalia
{
    public float Score { get; set; }
}

public class TareaAreaInput
{
    [LoadColumn(0)] public string Area { get; set; } = "";
    [LoadColumn(1)] public string Descripcion { get; set; } = "";
    [LoadColumn(2)] public float TiempoMinutos { get; set; }
    [LoadColumn(3)] public float FrecuenciaMensual { get; set; }
    [LoadColumn(4)] public float NumeroPasos { get; set; }
    [LoadColumn(5)] public float CantidadHerramientas { get; set; }
    [LoadColumn(6)] public string TipoProblema { get; set; } = "";
    [LoadColumn(7)] public bool EsAutomatizable { get; set; }
}

public class PrediccionTipo
{
    [ColumnName("PredictedLabel")] public string TipoProblema { get; set; } = "";
    public float[] Score { get; set; } = Array.Empty<float>();
}

// único servicio que usa ML.NET
public class AnalisisMLService
{
    private const int MinimoFilas = 50;   // V14

    private readonly MLContext _ml = new(seed: 1);
    private ITransformer? _modeloAutomatizable;
    private ITransformer? _modeloAtipicos;
    private float _umbralAtipico;
    private ITransformer? _modeloTipo;
    private Dictionary<(string Area, string Tipo), double> _promedioTiempo = new();

    // métricas del último entrenamiento (para la demo)
    public string ResumenEntrenamiento { get; private set; } = "Sin entrenar";
    public string ResumenTipoProblema { get; private set; } = "Sin entrenar";

    public void EntrenarModelos(string rutaDataset)
    {
        IDataView datos = _ml.Data.LoadFromTextFile<TareaInput>(
            rutaDataset, separatorChar: ',', hasHeader: true);

        int filas = _ml.Data
            .CreateEnumerable<TareaInput>(datos, reuseRowObject: false)
            .Count();
        if (filas < MinimoFilas)
            throw new InvalidOperationException(
                $"El dataset tiene {filas} filas; se necesitan al menos {MinimoFilas}.");

        // Modelo 1: clasificación binaria "¿la tarea es automatizable?"
        var particion = _ml.Data.TrainTestSplit(datos, testFraction: 0.2, seed: 1);

        var pipelineClasificacion = _ml.Transforms
            .Concatenate("Features",
                nameof(TareaInput.TiempoMinutos),
                nameof(TareaInput.FrecuenciaMensual),
                nameof(TareaInput.NumeroPasos),
                nameof(TareaInput.CantidadHerramientas))
            .Append(_ml.Transforms.NormalizeMinMax("Features"))
            .Append(_ml.BinaryClassification.Trainers.SdcaLogisticRegression(
                labelColumnName: nameof(TareaInput.EsAutomatizable),
                featureColumnName: "Features"));

        _modeloAutomatizable = pipelineClasificacion.Fit(particion.TrainSet);

        var m = _ml.BinaryClassification.Evaluate(
            _modeloAutomatizable.Transform(particion.TestSet),
            labelColumnName: nameof(TareaInput.EsAutomatizable));

        ResumenEntrenamiento =
            $"Exactitud: {m.Accuracy:P1} | AUC: {m.AreaUnderRocCurve:F3} | " +
            $"F1: {m.F1Score:F3}";

        // Modelo 2: detección de tiempos atípicos (no necesita etiquetas)
        var pipelineAtipicos = _ml.Transforms
            .Concatenate("Features",
                nameof(TareaInput.TiempoMinutos),
                nameof(TareaInput.FrecuenciaMensual),
                nameof(TareaInput.NumeroPasos))
            .Append(_ml.Transforms.NormalizeMinMax("Features"))
            .Append(_ml.AnomalyDetection.Trainers.RandomizedPca(
                featureColumnName: "Features", rank: 2));

        _modeloAtipicos = pipelineAtipicos.Fit(datos);

        // Umbral de "atípico": percentil 95 de los puntajes del entrenamiento
        var vistaPuntajes = _modeloAtipicos.Transform(datos);
        var puntajes = _ml.Data
            .CreateEnumerable<PuntajeAnomalia>(vistaPuntajes, reuseRowObject: false)
            .Select(p => p.Score)
            .OrderBy(p => p)
            .ToList();
        _umbralAtipico = puntajes[(int)(puntajes.Count * 0.95)];
    }

    public PrediccionTarea PredecirAutomatizable(Tarea tarea)
    {
        if (_modeloAutomatizable is null)
            throw new InvalidOperationException("Primero hay que entrenar los modelos.");

        var motor = _ml.Model
            .CreatePredictionEngine<TareaInput, PrediccionTarea>(_modeloAutomatizable);
        return motor.Predict(AEntrada(tarea));
    }

    public List<TareaAtipica> DetectarTiemposAtipicos(List<Tarea> tareas)
    {
        if (_modeloAtipicos is null)
            throw new InvalidOperationException("Primero hay que entrenar los modelos.");

        var entradas = tareas.Select(AEntrada).ToList();
        var vista = _modeloAtipicos.Transform(_ml.Data.LoadFromEnumerable(entradas));
        var puntajes = _ml.Data
            .CreateEnumerable<PuntajeAnomalia>(vista, reuseRowObject: false)
            .ToList();

        var resultado = new List<TareaAtipica>();
        for (int i = 0; i < tareas.Count; i++)
        {
            resultado.Add(new TareaAtipica
            {
                TareaId = tareas[i].Id,
                TiempoMinutos = tareas[i].TiempoMinutos,
                Puntaje = puntajes[i].Score,
                EsAtipica = puntajes[i].Score > _umbralAtipico
            });
        }
        return resultado;
    }

    public void EntrenarModeloTipoProblema(string rutaDataset)
    {
        IDataView datos = _ml.Data.LoadFromTextFile<TareaAreaInput>(
            rutaDataset, separatorChar: ',', hasHeader: true);

        _promedioTiempo = _ml.Data
            .CreateEnumerable<TareaAreaInput>(datos, reuseRowObject: false)
            .GroupBy(f => (f.Area, f.TipoProblema))
            .ToDictionary(g => g.Key, g => g.Average(f => (double)f.TiempoMinutos));

        var particion = _ml.Data.TrainTestSplit(datos, testFraction: 0.2, seed: 1);

        var pipeline = _ml.Transforms.Conversion
            .MapValueToKey("Label", nameof(TareaAreaInput.TipoProblema))
            .Append(_ml.Transforms.Text.FeaturizeText("TextoFeatures", nameof(TareaAreaInput.Descripcion)))
            .Append(_ml.Transforms.Concatenate("NumFeatures",
                nameof(TareaAreaInput.TiempoMinutos),
                nameof(TareaAreaInput.FrecuenciaMensual),
                nameof(TareaAreaInput.NumeroPasos),
                nameof(TareaAreaInput.CantidadHerramientas)))
            .Append(_ml.Transforms.NormalizeMinMax("NumFeatures"))
            .Append(_ml.Transforms.Concatenate("Features", "TextoFeatures", "NumFeatures"))
            .Append(_ml.MulticlassClassification.Trainers.SdcaMaximumEntropy("Label", "Features"))
            .Append(_ml.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

        _modeloTipo = pipeline.Fit(particion.TrainSet);

        var m = _ml.MulticlassClassification.Evaluate(
            _modeloTipo.Transform(particion.TestSet), labelColumnName: "Label");

        ResumenTipoProblema =
            $"Exactitud: {m.MicroAccuracy:P1} | Exactitud media por clase: {m.MacroAccuracy:P1}";
    }

    public double? PromedioTiempo(string? area, string tipo)
    {
        if (area != null && _promedioTiempo.TryGetValue((area, tipo), out var promedio))
            return promedio;
        return null;
    }

    public (string Tipo, float Confianza) PredecirTipoProblema(Tarea tarea)
    {
        if (_modeloTipo is null)
            throw new InvalidOperationException("Primero hay que entrenar el modelo de tipo de problema.");

        var motor = _ml.Model.CreatePredictionEngine<TareaAreaInput, PrediccionTipo>(_modeloTipo);
        var e = AEntrada(tarea);
        var p = motor.Predict(new TareaAreaInput
        {
            Descripcion = tarea.Descripcion,
            TiempoMinutos = e.TiempoMinutos,
            FrecuenciaMensual = e.FrecuenciaMensual,
            NumeroPasos = e.NumeroPasos,
            CantidadHerramientas = e.CantidadHerramientas
        });
        return (p.TipoProblema, p.Score.Length > 0 ? p.Score.Max() : 0f);
    }

    private static TareaInput AEntrada(Tarea t)
    {
        var opciones = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries;
        int herramientas = string.IsNullOrWhiteSpace(t.Herramientas)
            ? 0
            : t.Herramientas.Split(',', opciones).Length;

        return new TareaInput
        {
            TiempoMinutos = t.TiempoMinutos,
            FrecuenciaMensual = t.FrecuenciaMensual,
            NumeroPasos = t.NumeroPasos,
            CantidadHerramientas = herramientas
        };
    }
}