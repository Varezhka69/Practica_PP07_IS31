using System.Windows.Forms;

namespace SmartphoneDefectsDatabase1
{
    public partial class ChangePasswordForm : Form
    {
        private User user;
        private TextBox txtNewPassword, txtConfirmPassword;

        public ChangePasswordForm(User changeUser)
        {
            user = changeUser;
            InitializeCustomComponents();
        }

        private void InitializeCustomComponents()
        {
            this.Text = $"Смена пароля для {user.Username}";
            this.Size = new System.Drawing.Size(400, 200);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            var mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 3,
                Padding = new Padding(20)
            };

            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150F));
            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 35F));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 35F));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));

            mainPanel.Controls.Add(new Label
            {
                Text = "Новый пароль:",
                Dock = DockStyle.Fill,
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            }, 0, 0);

            txtNewPassword = new TextBox
            {
                Dock = DockStyle.Fill,
                UseSystemPasswordChar = true
            };
            mainPanel.Controls.Add(txtNewPassword, 1, 0);

            mainPanel.Controls.Add(new Label
            {
                Text = "Подтверждение:",
                Dock = DockStyle.Fill,
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            }, 0, 1);

            txtConfirmPassword = new TextBox
            {
                Dock = DockStyle.Fill,
                UseSystemPasswordChar = true
            };
            mainPanel.Controls.Add(txtConfirmPassword, 1, 1);

            var buttonPanel = new Panel { Dock = DockStyle.Fill };

            var btnOK = new Button
            {
                Text = "OK",
                DialogResult = DialogResult.OK,
                Location = new System.Drawing.Point(150, 10),
                Size = new System.Drawing.Size(80, 30)
            };

            var btnCancel = new Button
            {
                Text = "Отмена",
                DialogResult = DialogResult.Cancel,
                Location = new System.Drawing.Point(240, 10),
                Size = new System.Drawing.Size(80, 30)
            };

            btnOK.Click += (s, e) => { SavePassword(); };

            buttonPanel.Controls.Add(btnOK);
            buttonPanel.Controls.Add(btnCancel);

            mainPanel.Controls.Add(buttonPanel, 0, 2);
            mainPanel.SetColumnSpan(buttonPanel, 2);

            this.Controls.Add(mainPanel);
            this.AcceptButton = btnOK;
            this.CancelButton = btnCancel;
        }

        private void SavePassword()
        {
            if (string.IsNullOrWhiteSpace(txtNewPassword.Text))
            {
                MessageBox.Show("Введите новый пароль", "Ошибка");
                txtNewPassword.Focus();
                this.DialogResult = DialogResult.None;
                return;
            }

            if (txtNewPassword.Text != txtConfirmPassword.Text)
            {
                MessageBox.Show("Пароли не совпадают", "Ошибка");
                txtConfirmPassword.Focus();
                this.DialogResult = DialogResult.None;
                return;
            }

            if (txtNewPassword.Text.Length < 4)
            {
                MessageBox.Show("Пароль должен содержать минимум 4 символа", "Ошибка");
                txtNewPassword.Focus();
                this.DialogResult = DialogResult.None;
                return;
            }

            user.Password = txtNewPassword.Text;
        }
    }
}
