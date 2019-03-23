using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using ArticlesClassifactionCore;
using ArticlesClassifactionCore.Data;
using ArticlesClassifactionCore.Features;
using ArticlesClassifactionCore.Metrics;
using ArticlesClassifactionCore.SimilarityFunctions;
using ArticlesClassificationView.ViewModels.Base;
using Microsoft.Win32;

namespace ArticlesClassificationView.ViewModels
{
    public class MainWindowVM : BaseViewModel
    {
        private string _filter;
        public List<string> Categories { get; set; }
        public string SelectedCategory { get; set; }
        public List<ArticleData> Articles { get; set; }
        public List<ArticleData> FilteredArticles { get; set; }
        public List<PreprocessedArticle> TrainingData { get; set; }
        public List<PreprocessedArticle> TestData { get; set; }
        public ObservableCollection<TagVM> Tags { get; set; }
        public CollectionView TagsFiltered { get; set; }
        public List<string> StopList { get; set; }
        public int SliderValue { get; set; }
        public TrainingService TrainingService { get; set; }
        public KnnService KnnService { get; set; }
        public List<string> SelectedTags { get; set; }

        #region Commands
        public ICommand LoadFilesCommand { get; set; }
        public ICommand SelectionChanged { get; set; }
        public ICommand FilterDataCommand { get; set; }
        public ICommand CreateDataCommand { get; set; }
        public ICommand LoadStopListCommand { get; set; }
        public ICommand TrainCommand { get; set; }
        public ICommand ClassifyCommand { get; set; }
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

        public MainWindowVM()
        {
            SliderValue = 60;
            Tags = new ObservableCollection<TagVM>();
            Categories = new List<string>();
            Articles = new List<ArticleData>();
            StopList = new List<string>();
            LoadFilesCommand = new RelayCommand(LoadFiles);
            SelectionChanged = new RelayCommand(LoadTags);
            FilterDataCommand = new RelayCommand(FilterData);
            CreateDataCommand = new RelayCommand(CreateData);
            LoadStopListCommand = new RelayCommand(LoadStopList);
            TrainCommand = new RelayCommand(Train);
            ClassifyCommand = new RelayCommand(Classify);
        }

        public void Train()
        {
            TrainingService = new TrainingService(SelectedCategory, SelectedTags, TrainingData);
            TrainingService.Train();
        }

        public void Classify()
        {
            KnnService = new KnnService(new FeaturesVectorService(TrainingService.KeyWords, new IsEqualOrNot()), new EuclideanMetric(), 5);
            List<PreprocessedArticle> articles = new List<PreprocessedArticle>();
            foreach (var pro in SelectedTags)
            {
                var temp = TestData.Where(t => t.Label == pro).Take(5).ToList();
                articles.AddRange(temp);
                foreach (PreprocessedArticle article in temp)
                {
                    TestData.Remove(article);
                }
            }
            KnnService.InitKnn(articles);
            int count = 0;
            foreach (PreprocessedArticle preprocessedArticle in TestData)
            {
                var test = KnnService.ClassifyArticle(preprocessedArticle);
                if (preprocessedArticle.Label == test)
                    count++;
            }
        }
        public void LoadStopList()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Environment.CurrentDirectory;
            openFileDialog.Filter = "Text files (*.txt)|*.txt";
            if (openFileDialog.ShowDialog() == true)
            {
                StopList = File.ReadAllLines(openFileDialog.FileName).ToList();
            }
        }
        public void CreateData()
        {
            TrainingData = FilteredArticles.Take((int)(FilteredArticles.Count * SliderValue / 100.0)).Select(t => new PreprocessedArticle(t, t.Tags[SelectedCategory][0], StopList)).ToList();
            TestData = FilteredArticles.Skip((int)(FilteredArticles.Count * SliderValue / 100.0)).Select(t => new PreprocessedArticle(t, t.Tags[SelectedCategory][0], StopList)).ToList();

        }
        public void FilterData()
        {
            SelectedTags = Tags.Where(s => s.IsChecked).Select(t => t.Name).ToList();
            FilteredArticles = DataUtils.Filter(Articles, SelectedCategory,
               SelectedTags);
        }
        private bool FilterTags(object item)
        {
            if (String.IsNullOrEmpty(Filter))
                return true;
            return ((TagVM)item).Name.IndexOf(Filter, StringComparison.OrdinalIgnoreCase) >= 0;
        }
        public void LoadFiles()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.InitialDirectory = Environment.CurrentDirectory;
            openFileDialog.Filter = "SGML files (*.sgm)|*.sgm";
            if (openFileDialog.ShowDialog() == true)
            {
                foreach (string filename in openFileDialog.FileNames)
                {
                    SgmParser parser = new SgmParser();
                    Articles.AddRange(parser.FromSgml(Path.GetFullPath(filename)));
                }

                Categories = DataUtils.GetCategories(Articles);
                if (Categories != null && Categories.Count > 0)
                    SelectedCategory = Categories[0];
            }
        }

        public void LoadTags()
        {
            Tags = new ObservableCollection<TagVM>(DataUtils.GetTags(Articles, SelectedCategory).Select(t => new TagVM() { IsChecked = false, Name = t }));
            TagsFiltered = (CollectionView)CollectionViewSource.GetDefaultView(Tags);
            TagsFiltered.Filter = FilterTags;
        }
    }
}
