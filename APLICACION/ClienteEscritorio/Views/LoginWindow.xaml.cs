using System.Windows;

namespace ClienteEscritorio.Views
{
    public partial class LoginWindow : Window
    {
        private const string USUARIO_VALIDO = "MONSTER";
        private const string PASSWORD_VALIDA = "monster9";

        public LoginWindow()
        {
            InitializeComponent();
        }

        private void BtnIngresar_Click(object sender, RoutedEventArgs e)
        {
            string usuario = TxtUsuario.Text.Trim();
            string password = TxtPassword.Password;

            if (usuario == USUARIO_VALIDO && password == PASSWORD_VALIDA)
            {
                // Login exitoso
                ErrorPanel.Visibility = Visibility.Collapsed;
                
                var menuWindow = new MenuWindow();
                menuWindow.Show();
                this.Close();
            }
            else
            {
                // Mostrar error
                ErrorPanel.Visibility = Visibility.Visible;
                TxtPassword.Clear();
            }
        }

        private void BtnSalir_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
