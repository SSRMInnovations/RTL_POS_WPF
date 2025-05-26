using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using RTL_POS_WPF.Entities;

namespace RTL_POS_WPF
{
    public partial class MainWindow : Window
    {
        #region Fields
        private List<Customer> customers;
        private List<Category> categories;
        private List<Product> products;
        private List<PaymentMethod> paymentMethods;

        private ObservableCollection<OrderItem> currentOrder = new();
        private decimal totalAmount = 0;

        private Button selectedCustomerButton = null;
        private Button selectedProductButton = null;
        private Button selectedCategoryButton = null;
        private Button selectedPaymentButton = null;
        private DispatcherTimer clockTimer;

        private string selectedCustomerId;
        private List<OrderItem> submittedOrderItems;
        private string selectedPaymentMethodId;

        #endregion Fields

        public MainWindow()
        {
            InitializeComponent();
            InitializeDummyData();
            LoadCustomers();
            LoadCategories();
            OrderDataGrid.ItemsSource = currentOrder;
            LoadPaymentOptions();
            StartClock();
        }

        #region Initialize static/dummy data
        private void StartClock()
        {
            clockTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            clockTimer.Tick += (s, e) =>
            {
                if (CurrentTimeTextBlock != null)
                    CurrentTimeTextBlock.Text = DateTime.Now.ToString("yyyy-MM-dd  HH:mm:ss");
            };
            clockTimer.Start();
        }

        private void InitializeDummyData()
        {
            CategoryListBox.Items.Clear();
            ProductItemsControl.Items.Clear();
            customers = new List<Customer>()
            {
                new Customer{ Id = "uber", Name="Uber" },
                new Customer{ Id = "careem", Name="Careem" },
                new Customer{ Id = "talabat", Name="Talabat" },
                new Customer{ Id = "dunzo", Name="Dunzo" },
                new Customer{ Id = "cust1", Name="Customer1" },
                new Customer{ Id = "cust2", Name="Customer2" }

            };

            categories = new List<Category>
            {
                new Category{ Id = 1, Name="Appetizers (Mezze)" },
                new Category{ Id = 2, Name="Bread & Bakery" },
                new Category{ Id = 3, Name="Soups" },
                new Category{ Id = 4, Name="Main Dishes" },
                new Category{ Id = 5, Name="Seafood" },
                new Category{ Id = 6, Name="Chicken & Meat Specialties" },
                new Category{ Id = 7, Name="Side Dishes" },
                new Category{ Id = 8, Name="Salad" }
            };

            products = new List<Product>
            {
                new Product{ Id=1, CategoryId=1, Name="Hummus", Price=10 },
                new Product{ Id=2, CategoryId=1, Name="Mutabbal", Price=10 },
                new Product{ Id=3, CategoryId=1, Name="Dolma", Price=10 },
                new Product{ Id=4, CategoryId=1, Name="Falafel", Price=10 },

                new Product{ Id=5, CategoryId=2, Name="Samoon", Price=8 },
                new Product{ Id=6, CategoryId=2, Name="Pita bread", Price=15 },
                new Product{ Id=7, CategoryId=2, Name="Fatayer", Price=12 },

                new Product{ Id=8, CategoryId=3, Name="Shorbat Addas", Price=8 },
                new Product{ Id=9, CategoryId=3, Name="Chicken Soup", Price=15 },
                new Product{ Id=10, CategoryId=3, Name="Veg Soup", Price=12 },

                new Product{ Id=11, CategoryId=4, Name="Mashawi", Price=8 },
                new Product{ Id=12, CategoryId=4, Name="Maraq or Tashreeb", Price=15 },
                new Product{ Id=13, CategoryId=4, Name="Rice", Price=12 },

                new Product{ Id=14, CategoryId=5, Name="Seafood1", Price=8 },
                new Product{ Id=15, CategoryId=5, Name="Seafood2", Price=15 }
            };

            paymentMethods = new List<PaymentMethod>
            {
                new PaymentMethod{ Id="cash", Name="Cash" },
                new PaymentMethod{ Id="paypal", Name="PayPal" },
                new PaymentMethod{ Id="visa", Name="Visa" },
                new PaymentMethod{ Id="mastercard", Name="MasterCard" }
            };
        }

        private void LoadCustomers()
        {
            CustomersPanel.Items.Clear();
            foreach (var customer in customers)
            {
                var btn = new Button
                {
                    Tag = customer.Id,
                    ToolTip = customer.Name,
                    Style = (Style)FindResource("CustomerButtonStyle")
                };

                var img = new Image
                {
                    Source = new BitmapImage(new Uri($"Resources/{customer.Id}.png", UriKind.Relative)),
                    Width = 48,
                    Height = 48,
                    Stretch = Stretch.Uniform
                };

                btn.Content = img;
                btn.Click += CustomerButton_Click;
                CustomersPanel.Items.Add(btn);
            }
        }

        private void LoadCategories()
        {
            CategoryListBox.Items.Clear();
            foreach (var category in categories)
            {
                var btn = new Button
                {
                    Content = category.Name,
                    Tag = category.Id,
                    Style = (Style)FindResource("CategoryButtonStyle")
                };
                btn.Click += CategoryButton_Click;
                CategoryListBox.Items.Add(btn);
            }
            // Optionally select the first category
            if (CategoryListBox.Items.Count > 0)
            {
                var firstBtn = CategoryListBox.Items[0] as Button;
                SetIsCategorySelected(firstBtn, true);
                selectedCategoryButton = firstBtn;
            }
        }

        private void LoadProducts(List<Product> productList)
        {
            ProductItemsControl.Items.Clear();
            foreach (var product in productList)
            {
                var btn = new Button
                {
                    Content = $"{product.Name} ({product.Price:C0})",
                    Tag = product,
                    Style = (Style)FindResource("ProductButtonStyle")
                };
                btn.Click += ProductButton_Click;
                ProductItemsControl.Items.Add(btn);
            }
        }

        private void LoadPaymentOptions()
        {
            PaymentOptionsPanel.Items.Clear();
            foreach (var method in paymentMethods)
            {
                var btn = new Button
                {
                    Tag = method,
                    Style = (Style)FindResource("PaymentButtonStyle")
                };

                // StackPanel for image and text
                var stack = new StackPanel { Orientation = Orientation.Vertical, HorizontalAlignment = HorizontalAlignment.Center };
                var img = new Image
                {
                    Width = 40,
                    Height = 40,
                    Stretch = Stretch.Uniform
                };
                try
                {
                    //img.Source = new BitmapImage(new Uri($"pack://application:,,,/Images/{method.Id}.jpg"));
                    img.Source = new BitmapImage(new Uri($"Resources/{method.Id}.png", UriKind.Relative));

                }
                catch
                {
                    // Optionally set a fallback image or leave blank
                }
                var txt = new TextBlock
                {
                    Text = method.Name,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(0, 4, 0, 0)
                };
                stack.Children.Add(img);
                stack.Children.Add(txt);

                btn.Content = stack;
                btn.Click += PaymentButton_Click;
                PaymentOptionsPanel.Items.Add(btn);
            }
        }

        #endregion Initialize static/dummy data

        private void CategoryButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedCategoryButton != null)
                SetIsCategorySelected(selectedCategoryButton, false);

            selectedCategoryButton = sender as Button;
            SetIsCategorySelected(selectedCategoryButton, true);

            // Load products for this category
            if (selectedCategoryButton.Tag is int categoryId)
            {
                var filteredProducts = products.Where(p => p.CategoryId == categoryId).ToList();
                LoadProducts(filteredProducts);
            }
        }

        private void ProductButton_Click(object sender, RoutedEventArgs e)
        {
            // Visual selection logic
            if (selectedProductButton != null)
                SetIsProductSelected(selectedProductButton, false);

            selectedProductButton = sender as Button;
            SetIsProductSelected(selectedProductButton, true);

            // Existing logic for adding to order
            if (sender is Button btn && btn.Tag is Product product)
            {
                string modifiers = PromptForModifiers(product.Name);
                var existing = currentOrder.FirstOrDefault(i => i.Name == product.Name && i.Modifiers == modifiers);
                if (existing != null)
                {
                    existing.Quantity++;
                }
                else
                {
                    currentOrder.Add(new OrderItem
                    {
                        Name = product.Name,
                        Quantity = 1,
                        Modifiers = modifiers,
                        Price = product.Price
                    });
                }
                OrderDataGrid.Items.Refresh();
                UpdateTotal();
            }
        }

        private string PromptForModifiers(string productName)
        {
            // Simple WPF input dialog for modifiers
            //var inputWindow = new ModifierInputWindow(productName);
            //if (inputWindow.ShowDialog() == true)
            //{
            //    return inputWindow.Modifiers;
            //}
            return string.Empty;
        }

        private void ModifierTextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBlock tb && tb.DataContext is OrderItem item)
            {
                var inputWindow = new ModifierInputWindow(item.Name)
                {
                    Owner = this
                };
                inputWindow.ModifiersTextBox.Text = item.Modifiers;
                if (inputWindow.ShowDialog() == true)
                {
                    item.Modifiers = inputWindow.Modifiers;
                    OrderDataGrid.Items.Refresh();
                }
            }
        }

        private void DeleteOrderItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is OrderItem item)
            {
                currentOrder.Remove(item);
                UpdateTotal();
            }
        }

        private void PaymentButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedPaymentButton != null)
                SetIsPaymentSelected(selectedPaymentButton, false);

            selectedPaymentButton = sender as Button;
            SetIsPaymentSelected(selectedPaymentButton, true);

            if (selectedPaymentButton.Tag is PaymentMethod method)
            {
                // Store selected payment method ID
                selectedPaymentMethodId = method.Id;

                // Store a copy of the current order items (with notes/modifiers)
                submittedOrderItems = currentOrder
                    .Select(item => new OrderItem
                    {
                        Name = item.Name,
                        Quantity = item.Quantity,
                        Modifiers = item.Modifiers,
                        Price = item.Price
                    })
                    .ToList();

                // Create order summary object
                var orderSummary = new OrderSummary
                {
                    CustomerId = selectedCustomerId,
                    PaymentMethodId = selectedPaymentMethodId,
                    Items = submittedOrderItems,
                    Total = totalAmount,
                    Timestamp = DateTime.Now
                };

                ShowOrderSummary(orderSummary);
                SaveOrderToFile(orderSummary);

                // Clear order after payment
                currentOrder.Clear();
                UpdateTotal();
            }
        }

        private void IncreaseQuantity_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is OrderItem item)
            {
                item.Quantity++;
                OrderDataGrid.Items.Refresh();
                UpdateTotal();
            }
        }

        private void DecreaseQuantity_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is OrderItem item)
            {
                if (item.Quantity > 1)
                {
                    item.Quantity--;
                }
                else
                {
                    // Optionally remove the item if quantity reaches 0 or 1
                    currentOrder.Remove(item);
                }
                OrderDataGrid.Items.Refresh();
                UpdateTotal();
            }
        }

        private void CustomerButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedCustomerButton != null)
                SetIsCustomerSelected(selectedCustomerButton, false);

            selectedCustomerButton = sender as Button;
            SetIsCustomerSelected(selectedCustomerButton, true);

            // Store selected customer ID
            selectedCustomerId = selectedCustomerButton.Tag as string;
        }

        private void ShowOrderSummary(OrderSummary orderSummary)
        {
            string summaryText =
                $"Order Summary\n" +
                $"----------------------\n" +
                $"Customer: {orderSummary.CustomerId ?? "None"}\n" +
                $"Payment: {orderSummary.PaymentMethodId}\n" +
                $"Total: {orderSummary.Total:C2}\n" +
                $"Time: {orderSummary.Timestamp:yyyy-MM-dd HH:mm:ss}\n" +
                $"Items:\n" +
                string.Join("\n", orderSummary.Items.Select(i =>
                    $"- {i.Name} x{i.Quantity} {(string.IsNullOrWhiteSpace(i.Modifiers) ? "" : $"[{i.Modifiers}]")} @ {i.Price:C2}"));

            MessageBox.Show(summaryText, "Order Summary", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void SaveOrderToFile(OrderSummary orderSummary)
        {
            try
            {
                string json = System.Text.Json.JsonSerializer.Serialize(orderSummary, new System.Text.Json.JsonSerializerOptions { WriteIndented = false });
                string folder = "Orders";
                string fileName = $"Order_{orderSummary.Timestamp:yyyyMMdd}.json";
                string filePath = System.IO.Path.Combine(folder, fileName);

                System.IO.Directory.CreateDirectory(folder);

                using (var sw = new System.IO.StreamWriter(filePath, append: true))
                {
                    sw.WriteLine(json);
                }

                MessageBox.Show($"Order appended to {filePath}", "Order Saved", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save order: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateTotal()
        {
            totalAmount = currentOrder.Sum(i => i.Price * i.Quantity);
            if (TotalTextBlock != null)
                TotalTextBlock.Text = $"{totalAmount:C2}";
        }

        int count = 0;
        private void ThemeDark_Click(object sender, RoutedEventArgs e)
        {
            if (count == 0)
                ThemeManager.ApplyThemeToWindow(this, "Themes/Styles-DarkModern.xaml");
            else if (count == 1)
                ThemeManager.ApplyThemeToWindow(this, "Themes/Styles-BlueModern.xaml");
            else if (count == 2)
                ThemeManager.ApplyThemeToWindow(this, "Themes/Styles.xaml");
            //else if (count == 3)
            //    ThemeManager.ApplyThemeToWindow(this, "Themes/Styles-Blue.xaml");
            count++;
            if (count > 3) count = 0; // Reset count after 4 clicks

        }

        private void ThemeBlue_Click(object sender, RoutedEventArgs e)
        {
            ThemeManager.ApplyThemeToWindow(this, "Themes/Styles-BlueModern.xaml");
        }

        #region Attached Properties

        public static readonly DependencyProperty IsCustomerSelectedProperty =
            DependencyProperty.RegisterAttached(
                "IsCustomerSelected",
                typeof(bool),
                typeof(MainWindow),
                new PropertyMetadata(false));

        public static bool GetIsCustomerSelected(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsCustomerSelectedProperty);
        }
        public static void SetIsCustomerSelected(DependencyObject obj, bool value)
        {
            obj.SetValue(IsCustomerSelectedProperty, value);
        }

        public static readonly DependencyProperty IsProductSelectedProperty =
            DependencyProperty.RegisterAttached(
                "IsProductSelected",
                typeof(bool),
                typeof(MainWindow),
                new PropertyMetadata(false));

        public static bool GetIsProductSelected(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsProductSelectedProperty);
        }
        public static void SetIsProductSelected(DependencyObject obj, bool value)
        {
            obj.SetValue(IsProductSelectedProperty, value);
        }

        public static readonly DependencyProperty IsCategorySelectedProperty =
            DependencyProperty.RegisterAttached(
                "IsCategorySelected",
                typeof(bool),
                typeof(MainWindow),
                new PropertyMetadata(false));

        public static bool GetIsCategorySelected(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsCategorySelectedProperty);
        }
        public static void SetIsCategorySelected(DependencyObject obj, bool value)
        {
            obj.SetValue(IsCategorySelectedProperty, value);
        }

        public static readonly DependencyProperty IsPaymentSelectedProperty =
            DependencyProperty.RegisterAttached(
                "IsPaymentSelected",
                typeof(bool),
                typeof(MainWindow),
                new PropertyMetadata(false));

        public static bool GetIsPaymentSelected(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsPaymentSelectedProperty);
        }
        public static void SetIsPaymentSelected(DependencyObject obj, bool value)
        {
            obj.SetValue(IsPaymentSelectedProperty, value);
        }

        #endregion Attached Properties

    }
}