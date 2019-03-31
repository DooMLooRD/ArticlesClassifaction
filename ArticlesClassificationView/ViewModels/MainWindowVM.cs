using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using ArticlesClassifactionCore;
using ArticlesClassifactionCore.Data;
using ArticlesClassifactionCore.Features;
using ArticlesClassifactionCore.Features.FeatureExtractors;
using ArticlesClassifactionCore.Metrics;
using ArticlesClassifactionCore.SimilarityFunctions;
using ArticlesClassificationView.ViewModels.Base;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using Parago.Windows;

namespace ArticlesClassificationView.ViewModels
{
    public class MainWindowVM : BaseViewModel
    {
        #region Fields

        private string _filter;
        private Window _owner;

        #endregion

        #region Props

        public List<string> Categories { get; set; }
        public string SelectedCategory { get; set; }
        public List<ArticleData> Articles { get; set; }
        public List<ArticleData> FilteredArticles { get; set; }
        public List<PreprocessedArticle> TrainingData { get; set; }
        public List<PreprocessedArticle> TestData { get; set; }
        public ObservableCollection<CheckBoxVM> Tags { get; set; }
        public CollectionView TagsFiltered { get; set; }
        public List<string> StopList { get; set; }
        public int SliderValue { get; set; }
        public TrainingService TrainingService { get; set; }
        public KnnService KnnService { get; set; }
        public List<string> SelectedTags { get; set; }
        public List<IMetric> Metrics { get; set; }
        public IMetric SelectedMetric { get; set; }
        public List<ISimilarityFunction> SimilarityFunctions { get; set; }
        public ISimilarityFunction SelectedSimilarityFunction { get; set; }
        public List<ExtractorVM> Extractors { get; set; }
        public List<string> KeyWordsExtractors { get; set; }
        public string SelectedKeyWordsExtractor { get; set; }
        public int KeyWordsCount { get; set; }
        public int ParamK { get; set; }
        public int ColdStartData { get; set; }
        #endregion

        #region Result Data

        public List<KeyWordVM> KeyWords { get; set; }
        public List<ResultVM> Results { get; set; }

        #endregion

        #region Booleans

        public bool IsLearned { get; set; }
        public bool IsLearning { get; set; }
        public bool IsDataLoading { get; set; }
        public bool IsDataLoaded { get; set; }
        public bool IsDataFiltered { get; set; }
        public bool IsStopListLoaded { get; set; }
        public bool IsDataPreprocessing { get; set; }
        public bool IsDataPreprocessed { get; set; }
        public bool IsClassifying { get; set; }
        public bool IsClassified { get; set; }

        #endregion

        #region Status

        public string DataStatus { get; set; } = "1. Data is not loaded.";
        public string FilterStatus { get; set; } = "2. Data is not filtered.";
        public string StopListStatus { get; set; } = "3. Stop List is not loaded.";
        public string PreprocessingStatus { get; set; } = "4. Data is not preprocessed.";
        public string LearnStatus { get; set; } = "5. Not learned.";
        public string ClassificationStatus { get; set; } = "6. Not classified.";


        #endregion

        #region Commands
        public ICommand LoadFilesCommand { get; set; }
        public ICommand SelectionChanged { get; set; }
        public ICommand FilterDataCommand { get; set; }
        public ICommand CreateDataCommand { get; set; }
        public ICommand LoadStopListCommand { get; set; }
        public ICommand TrainCommand { get; set; }
        public ICommand ClassifyCommand { get; set; }
        public ICommand ToggleBaseCommand { get; }
        #endregion


        public string Filter
        {
            get => _filter;
            set
            {
                _filter = value;
                CollectionViewSource.GetDefaultView(Tags).Refresh();
            }
        }

        public MainWindowVM(Window owner)
        {
            _owner = owner;
            SliderValue = 60;
            Tags = new ObservableCollection<CheckBoxVM>();
            Categories = new List<string>();
            Articles = new List<ArticleData>();
            StopList = new List<string>();
            KeyWords = new List<KeyWordVM>();
            Results = new List<ResultVM>();
            LoadFilesCommand = new RelayCommand(LoadFiles);
            SelectionChanged = new RelayCommand(LoadTags);
            FilterDataCommand = new RelayCommand(FilterData);
            CreateDataCommand = new RelayCommand(CreateData);
            LoadStopListCommand = new RelayCommand(LoadStopList);
            TrainCommand = new RelayCommand(Train);
            ClassifyCommand = new RelayCommand(Classify);
            ToggleBaseCommand = new RelayCommand<bool>(ApplyBase);
            KeyWordsExtractors = new List<string>() { "TermFrequency", "DocumentFrequency" };
            ParamK = 10;
            SelectedKeyWordsExtractor = KeyWordsExtractors[0];
            KeyWordsCount = 30;
            ColdStartData = 20;
            Metrics = new List<IMetric> { new EuclideanMetric(), new ChebyshevMetric(), new TaxicabMetric() };
            SelectedMetric = Metrics[0];
            SimilarityFunctions = new List<ISimilarityFunction> { new BinaryFunction(), new NGramFunction(4) };
            Extractors = new List<ExtractorVM>();
            SelectedSimilarityFunction = SimilarityFunctions[0];
        }

        public async void Train()
        {
            IsLearning = true;
            LearnStatus = "5. Learning...";
            TrainingService = new TrainingService(SelectedCategory, SelectedTags, TrainingData);
            await Task.Run(() =>
            {
                try
                {
                    KeyWords=new List<KeyWordVM>();
                    TrainingService.Train(SelectedKeyWordsExtractor, KeyWordsCount);
                    foreach (string tag in SelectedTags)
                    {
                        foreach (string s in TrainingService.KeyWords[tag])
                        {
                            KeyWords.Add(new KeyWordVM()
                            {
                                Tag = tag,
                                Word = s
                            });
                        }


                    }

                    IFeatureExtractor countExtractor = new CountOfKeyWordsExtractor(TrainingService.KeyWords);
                    IFeatureExtractor sumExtractor = new SumOfSimilarityArticleKeyWordsExtractor(TrainingService.KeyWords);
                    IFeatureExtractor articleExtractor = new ArticleExtractor();
                    Extractors = new List<ExtractorVM>()
                    {
                        new ExtractorVM()
                        {
                            FeatureExtractor = countExtractor,
                            IsChecked = true,
                            Features = countExtractor.Features.Select(c=> new CheckBoxVM(){Name = c.Name,IsChecked = c.IsChecked}).ToList()
                        },
                        new ExtractorVM()
                        {
                            FeatureExtractor = sumExtractor,
                            IsChecked = true,
                            Features = sumExtractor.Features.Select(c=> new CheckBoxVM(){Name = c.Name,IsChecked = c.IsChecked}).ToList()
                        },
                        new ExtractorVM()
                        {
                            FeatureExtractor = articleExtractor,
                            IsChecked = true,
                            Features = articleExtractor.Features.Select(c=> new CheckBoxVM(){Name = c.Name,IsChecked = c.IsChecked}).ToList()
                        }
                    };
                    LearnStatus = "5. Learned.";
                    IsLearned = true;

                }
                catch (Exception e)
                {
                    LearnStatus = "5. Not learned.";
                }

                IsLearning = false;

            });
        }
        private static void ApplyBase(bool isDark)
        {
            new PaletteHelper().SetLightDark(isDark);
        }
        public async void Classify()
        {
            IsClassifying = true;
            IsClassified = false;
            ClassificationStatus = "6. Classifying...";
            await Task.Run(() =>
                {
                    try
                    {
                        Results = new List<ResultVM>();
                        SelectedTags.ForEach(c => Results.Add(new ResultVM { Tag = c }));
                        KnnService = new KnnService(new FeaturesVectorService(TrainingService.KeyWords, SelectedSimilarityFunction, Extractors.Where(c => c.IsChecked).Select(
                                    t =>
                                    {
                                        var c = t.FeatureExtractor;
                                        c.Features = t.Features.Select(e => new Feature()
                                        { IsChecked = e.IsChecked, Name = e.Name }).ToList();
                                        return c;
                                    }).ToList()),
                                SelectedMetric, ParamK);
                        List<PreprocessedArticle> articles = new List<PreprocessedArticle>();
                        List<PreprocessedArticle> testArticles = TestData.ToList();
                        foreach (var pro in SelectedTags)
                        {
                            var temp = TestData.Where(t => t.Label == pro).Take(ColdStartData).ToList();
                            articles.AddRange(temp);
                            foreach (PreprocessedArticle article in temp)
                            {
                                testArticles.Remove(article);
                            }
                        }

                        KnnService.InitKnn(articles);
                        foreach (PreprocessedArticle preprocessedArticle in testArticles)
                        {
                            var predictedLabel = KnnService.ClassifyArticle(preprocessedArticle);
                            foreach (string key in SelectedTags)
                            {
                                if (preprocessedArticle.Label == key)
                                {
                                    Results.First(c => c.Tag == key).All++;
                                    if (predictedLabel == key)
                                        Results.First(c => c.Tag == key).Tp++;
                                }
                                else
                                {
                                    if (predictedLabel == key)
                                        Results.First(c => c.Tag == key).Tn++;
                                }

                            }
                        }

                        IsClassified = true;
                        ClassificationStatus = "6. Classified.";

                    }
                    catch (Exception e)
                    {
                        ClassificationStatus = "6. Not classified.";
                    }

                    IsClassifying = false;

                }
            );
        }
        public void LoadStopList()
        {

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Environment.CurrentDirectory;
            openFileDialog.Filter = "Text files (*.txt)|*.txt";
            if (openFileDialog.ShowDialog() == true)
            {
                StopList = File.ReadAllLines(openFileDialog.FileName).ToList();
                StopListStatus = "3. Stop List is loaded.";
                IsStopListLoaded = true;
            }
        }
        public async void CreateData()
        {
            PreprocessingStatus = "4. Preprocessing data...";
            IsDataPreprocessing = true;
            await Task.Run(() =>
            {
                try
                {
                    TrainingData = FilteredArticles.Take((int)(FilteredArticles.Count * SliderValue / 100.0)).Select(t => new PreprocessedArticle(t, t.Tags[SelectedCategory][0], StopList)).ToList();
                    TestData = FilteredArticles.Skip((int)(FilteredArticles.Count * SliderValue / 100.0)).Select(t => new PreprocessedArticle(t, t.Tags[SelectedCategory][0], StopList)).ToList();
                    PreprocessingStatus = "4. Data preprocessed.";
                    IsDataPreprocessed = true;
                }
                catch (Exception e)
                {
                    IsDataPreprocessed = false;
                    PreprocessingStatus = "4. Data is not preprocessed.";
                }

                IsDataPreprocessing = false;
            });
        }
        public void FilterData()
        {
            try
            {
                SelectedTags = Tags.Where(s => s.IsChecked).Select(t => t.Name).ToList();
                FilteredArticles = DataUtils.Filter(Articles, SelectedCategory,
                    SelectedTags).Where(c => c.Text.Body != null).ToList();
                FilterStatus = "2. Data is filtered.";
                IsDataFiltered = true;
            }
            catch (Exception e)
            {
                FilterStatus = "2. Data is not filtered.";
            }

        }
        private bool FilterTags(object item)
        {
            if (String.IsNullOrEmpty(Filter))
                return true;
            return ((CheckBoxVM)item).Name.IndexOf(Filter, StringComparison.OrdinalIgnoreCase) >= 0;
        }
        public async void LoadFiles()
        {
            DataStatus = "1. Loading Data...";
            IsDataLoading = true;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.InitialDirectory = Environment.CurrentDirectory;
            openFileDialog.Filter = "SGML files (*.sgm)|*.sgm";
            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    await Task.Run(() =>
                    {
                        foreach (string filename in openFileDialog.FileNames)
                        {
                            SgmParser parser = new SgmParser();
                            Articles.AddRange(parser.FromSgml(Path.GetFullPath(filename)));
                        }

                        Categories = DataUtils.GetCategories(Articles);
                        if (Categories != null && Categories.Count > 0)
                            SelectedCategory = Categories[0];
                    });
                    DataStatus = "1. Data is loaded.";
                    IsDataLoaded = true;
                }
                catch (Exception e)
                {
                    DataStatus = "1. Data is not loaded.";
                    IsDataLoading = false;
                }

            }
            else
            {
                DataStatus = "1. Data is not loaded.";
                IsDataLoading = false;
            }

        }

        public void LoadTags()
        {
            Tags = new ObservableCollection<CheckBoxVM>(DataUtils.GetTags(Articles, SelectedCategory).Select(t => new CheckBoxVM() { IsChecked = false, Name = t }));
            TagsFiltered = (CollectionView)CollectionViewSource.GetDefaultView(Tags);
            TagsFiltered.Filter = FilterTags;
        }
    }
}
