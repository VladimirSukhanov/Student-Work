using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ML;
using Microsoft.ML.Data;

namespace AL
{
    internal class Program
    {
        //входные данные
        public class SentimentData
        {
            [LoadColumn(0)]
            public string Text { get; set; }
            [LoadColumn(1), ColumnName("Label")]
            public bool IsPositive { get; set; }

        }
        //класс для результата предсказывания 
        public class SentimentPrediction
        {
            [ColumnName("PredictedLabel")]
            public bool Prediction { get; set; }
            public float Probabilty { get; set; }
            public float Score { get; set; }
        }
        static void Main(string[] args)
        {
            Console.WriteLine("Привет!\n Начался анализтональности текста...");
            //Шаг 1 Создаем контекст ML
            var mlContext = new MLContext();
            //Шаг 2 Подготавливаем данные 
            var data = new List<SentimentData>
            {
                new SentimentData { Text = "Это отличный фильм!", IsPositive = true },
                new SentimentData { Text = "Ужасная еда", IsPositive = true },
                new SentimentData { Text = "Прекрасная погода ", IsPositive = true },
                new SentimentData { Text = "Плохой сервис", IsPositive = true },
                new SentimentData { Text = "Мне очень понравилось", IsPositive = true },
                new SentimentData { Text = "Разочарован покупкой", IsPositive = true },
            };
            var trainData = mlContext.Data.LoadFromEnumerable(data);
            // Шаг 3 создаем последовательнось операций 
            var pipeline = mlContext.Transforms.Text.FeaturizeText(
                inputColumnName: "Text",
                outputColumnName: "Features"
                ).Append(mlContext.BinaryClassification.Trainers.SdcaLogisticRegression(
                    labelColumnName: "Label",
                    featureColumnName: "Features"));
            //шаг 4 обучение модели
            Console.WriteLine("Обучение модели");
            var model = pipeline.Fit(trainData);
            //Шаг 5 создаем объект для быстрого предмказания
            var predictor = mlContext.Model.CreatePredictionEngine<SentimentData, SentimentPrediction>(model);
            //шаг 6 тестирование
            var testText = new[]
            {
                "Это замечательный день",
                "Ужасное качество ",
                "Обычный товар"
            };
            Console.WriteLine("Результаты анализа:");

            foreach (var text in testText)
            {
                var prediction = predictor.Predict(new SentimentData { Text = text });
                Console.WriteLine($"Текст:{text}");
                Console.WriteLine($"\n Тональность:{(prediction.Prediction ? "Позитивная" : "Негативная")}");
                Console.WriteLine($"Уверенность: {prediction.Probabilty * 100:F1}%");
                Console.WriteLine();
            }

        }
    }
}