using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace БатталовГлазкиSave
{
    /// <summary>
    /// Логика взаимодействия для AddEditPage.xaml
    /// </summary>
    public partial class AddEditPage : Page
    {
        private Agent _currentAgent = new Agent();
        private ProductSale _currentProductSale = new ProductSale();
        public AddEditPage(Agent SelectedAgent)
        {
            InitializeComponent();
            if (SelectedAgent != null)
                _currentAgent = SelectedAgent;

            var _currentProductSaleForAgent = БатталовГлазкиSaveEntities.GetContext().ProductSale.Where(p => p.AgentID == _currentAgent.ID).ToList();
            ProductSaleListView.ItemsSource = _currentProductSaleForAgent;

            var productNames = БатталовГлазкиSaveEntities.GetContext().Product.Select(p => p.Title).ToList();
            ProductNameComboBox.ItemsSource = productNames;

            DataContext = _currentAgent;
        }



        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();
            if (string.IsNullOrWhiteSpace(_currentAgent.Title))
                errors.AppendLine("Укажите наименование агента");
            if (string.IsNullOrWhiteSpace(_currentAgent.Address))
                errors.AppendLine("Укажите адрес агента");
            if (string.IsNullOrWhiteSpace(_currentAgent.DirectorName))
                errors.AppendLine("Укажите ФИО директора");
            if (string.IsNullOrWhiteSpace(_currentAgent.Priority.ToString()))
                errors.AppendLine("Укажите приоритет агента");
            if (string.IsNullOrWhiteSpace(_currentAgent.INN))
                errors.AppendLine("Укажите ИНН агента");
            if (string.IsNullOrWhiteSpace(_currentAgent.KPP))
                errors.AppendLine("Укажите КПП агента");
            if (string.IsNullOrWhiteSpace(_currentAgent.Phone))
                errors.AppendLine("Укажите телефон агента");
            else
            {
                string ph = _currentAgent.Phone.Replace("(","").Replace("-","").Replace("+","");
                if (((ph[1] == '9' || ph[1] == '4' || ph[1] == '8') && ph.Length != 11) || (ph[1] == '3' && ph.Length != 12))
                    errors.AppendLine("Коректно введите телефон агента");
            }
            if (ComboType.SelectedItem == null)
                errors.AppendLine("Выберите тип агента");
            if (string.IsNullOrWhiteSpace(_currentAgent.Email))
                errors.AppendLine("Укажите email агента");
            if(errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }
            if(_currentAgent.ID == 0)
                БатталовГлазкиSaveEntities.GetContext().Agent.Add(_currentAgent);
            try
            {
                БатталовГлазкиSaveEntities.GetContext().SaveChanges();
                MessageBox.Show("Информация сохранена");
                Manager.MainFrame.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы уверены, что хотите удалить агента?", "Внимание", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    // Проверка на существование реализации продукции
                    if (_currentAgent.ProductSale.Any())
                    {
                        MessageBox.Show("Невозможно удалить агента, так как у него есть информация о реализации продукции.");
                        return;
                    }

                    // Удаление связанных данных
                    БатталовГлазкиSaveEntities.GetContext().AgentPriorityHistory.RemoveRange(_currentAgent.AgentPriorityHistory);
                    БатталовГлазкиSaveEntities.GetContext().Shop.RemoveRange(_currentAgent.Shop);

                    // Удаление самого агента
                    БатталовГлазкиSaveEntities.GetContext().Agent.Remove(_currentAgent);

                    // Сохранение изменений
                    БатталовГлазкиSaveEntities.GetContext().SaveChanges();

                    MessageBox.Show("Агент успешно удален.");
                    Manager.MainFrame.GoBack(); // Возврат к списку агентов
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении агента: {ex.Message}");
                }
            }
        }



        private void ChangeButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog myopenFileDialog = new OpenFileDialog();
            if (myopenFileDialog.ShowDialog() == true)
            {
                _currentAgent.Logo = myopenFileDialog.FileName;
                LogoImage.Source = new BitmapImage(new Uri(myopenFileDialog.FileName));
            }
        }
        private void ProductSale_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ProductSaleListView.SelectedItems.Count >= 1)
                DeleteProductSale.Visibility = Visibility.Visible;
            else
                DeleteProductSale.Visibility = Visibility.Hidden;
        }
        private void AddProductSale_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();
            if (ProductNameComboBox.SelectedIndex == -1)
                errors.AppendLine("Выберите наименование продукта");
            if (string.IsNullOrWhiteSpace(ProductCountTB.Text))
                errors.AppendLine("Укажите количество продукта");
            if (DatePickerProduct.SelectedDate == null || DatePickerProduct.SelectedDate.Value.Date > DateTime.Today)
                errors.AppendLine("Укажите дату продажи или дату, которая идет на предыдущие дни");

            if (errors.Length > 0)
                MessageBox.Show(errors.ToString());
            else
            {
                string selectedProductName = ProductNameComboBox.SelectedItem as string;
                var selectedProduct = БатталовГлазкиSaveEntities.GetContext().Product.FirstOrDefault(p => p.Title == selectedProductName);
                _currentProductSale.ProductID = selectedProduct.ID;

                _currentProductSale.AgentID = _currentAgent.ID;
                _currentProductSale.ProductCount = Convert.ToInt32(ProductCountTB.Text);
                _currentProductSale.SaleDate = DatePickerProduct.SelectedDate.Value;

                if (_currentProductSale.ID == 0)
                    БатталовГлазкиSaveEntities.GetContext().ProductSale.Add(_currentProductSale);

                try
                {
                    БатталовГлазкиSaveEntities.GetContext().SaveChanges();
                    UpdateProductSale();
                    MessageBox.Show("Информация сохранена");
                    ProductCountTB.Clear();
                    DatePickerProduct.SelectedDate = null;
                    ProductNameComboBox.SelectedIndex = -1;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }

        }

        private void UpdateProductSale()
        {
            var _currentProductSale = БатталовГлазкиSaveEntities.GetContext().ProductSale
                .Where(p => p.AgentID == _currentAgent.ID).ToList();

            //_currentProductSale = _currentProductSale.Where(p =>
            //    p.ProductName.ToLower().Contains(SearchTB.Text.ToLower())).ToList();

            ProductSaleListView.ItemsSource = _currentProductSale;
            ProductSaleListView.Items.Refresh();
        }
        private void DeleteProductSale_Click(object sender, RoutedEventArgs e)
        {
            var selected = ProductSaleListView.SelectedItems;

            if (selected.Count >= 1)
            {
                if (MessageBox.Show("Вы точно хотите выполнить удаление?", "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        foreach (var item in selected)
                        {
                            var productSaleToDelete = item as ProductSale; // Приводим объект к типу ProductSale
                            if (productSaleToDelete != null)
                            {
                                БатталовГлазкиSaveEntities.GetContext().ProductSale.Remove(productSaleToDelete);
                            }
                        }
                        БатталовГлазкиSaveEntities.GetContext().SaveChanges();
                        UpdateProductSale();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString());
                    }
                }
            }
            else
                MessageBox.Show("Выберите позиции!");

        }

        private void ProductSerch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = ProductSerch.Text.ToLower();

            var filteredProducts = БатталовГлазкиSaveEntities.GetContext().Product
                .Where(p => p.Title.ToLower().Contains(searchText))
                .Select(p => p.Title)
                .ToList();

            ProductNameComboBox.ItemsSource = filteredProducts;
        }
    }
}
