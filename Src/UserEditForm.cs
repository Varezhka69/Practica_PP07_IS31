using System.Linq;
using System.Windows.Forms;

namespace SmartphoneDefectsDatabase1
{
    public partial class UserEditForm : Form
    {
        private DefectContext dbContext;
        private User user;
        private TextBox txtUsername, txtFullName, txtEmail;
        private CheckBox chkIsActive;
        private ComboBox cmbRole;

        public UserEditForm(DefectContext context, User editUser)
        {
            dbContext = context;
            user = editUser;
            InitializeCustomComponents();
            LoadData();
        }

        private void InitializeCustomComponents()
        {
            this.Text = user.UserID == 0 ? "Добавление пользователя" : "Редактирование пользователя";
            this.Size = new System.Drawing.Size(500, 350);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            var mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 6,
                Padding = new Padding(20)
            };

            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150F));
            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            for (int i = 0; i < 5; i++)
            {
                mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 35F));
            }
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));

            mainPanel.Controls.Add(new Label
            {
                Text = "Логин:",
                Dock = DockStyle.Fill,
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            }, 0, 0);

            txtUsername = new TextBox { Dock = DockStyle.Fill };
            mainPanel.Controls.Add(txtUsername, 1, 0);

            mainPanel.Controls.Add(new Label
            {
                Text = "ФИО:",
                Dock = DockStyle.Fill,
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            }, 0, 1);

            txtFullName = new TextBox { Dock = DockStyle.Fill };
            mainPanel.Controls.Add(txtFullName, 1, 1);

            mainPanel.Controls.Add(new Label
            {
                Text = "Email:",
                Dock = DockStyle.Fill,
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            }, 0, 2);

            txtEmail = new TextBox { Dock = DockStyle.Fill };
            mainPanel.Controls.Add(txtEmail, 1, 2);

            mainPanel.Controls.Add(new Label
            {
                Text = "Роль:",
                Dock = DockStyle.Fill,
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            }, 0, 3);

            cmbRole = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
            mainPanel.Controls.Add(cmbRole, 1, 3);

            mainPanel.Controls.Add(new Label
            {
                Text = "Активен:",
                Dock = DockStyle.Fill,
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            }, 0, 4);

            chkIsActive = new CheckBox { Checked = true, Text = "" };
            mainPanel.Controls.Add(chkIsActive, 1, 4);

            var buttonPanel = new Panel { Dock = DockStyle.Fill };

            var btnOK = new Button
            {
                Text = "OK",
                DialogResult = DialogResult.OK,
                Location = new System.Drawing.Point(250, 10),
                Size = new System.Drawing.Size(80, 30)
            };

            var btnCancel = new Button
            {
                Text = "Отмена",
                DialogResult = DialogResult.Cancel,
                Location = new System.Drawing.Point(340, 10),
                Size = new System.Drawing.Size(80, 30)
            };

            btnOK.Click += (s, e) => { SaveData(); };

            buttonPanel.Controls.Add(btnOK);
            buttonPanel.Controls.Add(btnCancel);

            mainPanel.Controls.Add(buttonPanel, 0, 5);
            mainPanel.SetColumnSpan(buttonPanel, 2);

            this.Controls.Add(mainPanel);
            this.AcceptButton = btnOK;
            this.CancelButton = btnCancel;
        }

        private void LoadData()
        {
            var roles = dbContext.Roles.ToList();
            cmbRole.DataSource = roles;
            cmbRole.DisplayMember = "RoleName";
            cmbRole.ValueMember = "RoleID";

            if (user.UserID > 0)
            {
                txtUsername.Text = user.Username;
                txtFullName.Text = user.FullName;
                txtEmail.Text = user.Email;
                chkIsActive.Checked = user.IsActive;
                cmbRole.SelectedValue = user.RoleID;
            }
            else
            {
                user.Password = "123456"; // Пароль по умолчанию
            }
        }

        private void SaveData()
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                MessageBox.Show("Введите логин", "Ошибка");
                txtUsername.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                MessageBox.Show("Введите ФИО", "Ошибка");
                txtFullName.Focus();
                return;
            }

            user.Username = txtUsername.Text.Trim();
            user.FullName = txtFullName.Text.Trim();
            user.Email = txtEmail.Text.Trim();
            user.IsActive = chkIsActive.Checked;
            user.RoleID = (int)cmbRole.SelectedValue;
        }
    }
}
