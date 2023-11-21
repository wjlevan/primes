using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Primes
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private long num;  
        private bool runCalculation = false;
        private bool isPrime = false;
        private long inputVal;
        private bool isYellow = false;
        private Brush originalBackground;

        public MainWindow()
        {
            InitializeComponent();
            originalBackground = OutputBox.Background;
        }

        private void OutputBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            // Maintain the cursor position at the end of the text
            OutputBox.ScrollToEnd();
        }

        private void ToggleButton_OnClick(object sender, RoutedEventArgs e)
        {
            // On click, toggle between yellow and blue
            if (!isYellow)
            {
                isYellow = true;
                OutputBox.Background = Brushes.Yellow;
                OutputBox.Text += ", yellow";
            }
            else
            {
                isYellow = false;
                OutputBox.Background = Brushes.Blue;
                OutputBox.Text += ", blue";
            }

        }
        private void StartButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (!runCalculation)
            {
                // Run new calculation

                // Reset variables
                runCalculation = true;
                num = 1;
                inputVal = long.Parse(InputBox.Text);
                OutputBox.Background = originalBackground;
                OutputBox.Text = "";

                // Run calculation without blocking the UI thread
                StartButton.Dispatcher.InvokeAsync(CheckNextNumber, DispatcherPriority.SystemIdle);
            }
            else
            {
                MessageBox.Show("Calculation is running");
            }
        }

        private void TextBox_BeforeTextInput(object sender, TextCompositionEventArgs e)
        {
            // Only allow numbers to be entered
            if (!int.TryParse(e.Text, out _))
            {
                e.Handled = true; // Ignore the input if it's not a valid integer
            }
        }

        public void CheckNextNumber()
        {
            Task.Delay(TimeSpan.FromMilliseconds(100)).Wait(); // Artificial delay for User Experience

            // Check if calculation is done 
            if (num > inputVal)
            {
                runCalculation = false;
                OutputBox.Text += ", end";
                return;
            }

            // Edge cases for 1 and 2
            if (num == 1)
            {
                OutputBox.Text = "1";
                num++;
            }
            else if (num == 2)
            {
                OutputBox.Text += ", 2";
                num++;
            }
            else
            {
                // Only need to check up to the square root of the number
                long boundary = (long)Math.Sqrt(num);
                isPrime = true;
                for (long i = 3; i <= boundary; i++)
                {
                    if (num % i == 0)
                    {
                        isPrime = false;
                        break;
                    }
                }

                if (isPrime)
                    OutputBox.Text += ", " + num.ToString();

                num += 2;
            }

            if (runCalculation)
                // Check next number without blocking UI thread
                StartButton.Dispatcher.InvokeAsync(CheckNextNumber, DispatcherPriority.SystemIdle);
        }
    }
}