using System.Windows.Forms;

namespace SmartphoneDefectsDatabase
{
    public partial class RoleEditForm : Form
    {
        private Role role;
        private TextBox txtRoleName, txtDescription;

        public RoleEditForm(Role editRole)
        {
            role = editRole;
            InitializeCustomComponents();
            LoadData();
        }

        private void InitializeCustomComponents()
        {
            this.Text = role.RoleID == 0 ? "Добавление роли" : "Редактирование роли";
            this.Size = new System.Drawing.Size(500, 200);
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
                Text = "Название роли:",
                Dock = DockStyle.Fill,
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
                Font = new System.Drawing.Font("Microsoft Sans Serif", 9F)
            }, 0, 0);

            txtRoleName = new TextBox
            {
                Dock = DockStyle.Fill,
                Font = new System.Drawing.Font("Microsoft Sans Serif", 9F)
            };
            mainPanel.Controls.Add(txtRoleName, 1, 0);

            mainPanel.Controls.Add(new Label
            {
                Text = "Описание:",
                Dock = DockStyle.Fill,
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
                Font = new System.Drawing.Font("Microsoft Sans Serif", 9F)
            }, 0, 1);

            txtDescription = new TextBox
            {
                Dock = DockStyle.Fill,
                Font = new System.Drawing.Font("Microsoft Sans Serif", 9F)
            };
            mainPanel.Controls.Add(txtDescription, 1, 1);

            var buttonPanel = new Panel
            {
                Dock = DockStyle.Fill
            };

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

            mainPanel.Controls.Add(buttonPanel, 0, 2);
            mainPanel.SetColumnSpan(buttonPanel, 2);

            this.Controls.Add(mainPanel);
            this.AcceptButton = btnOK;
            this.CancelButton = btnCancel;
        }

        private void LoadData()
        {
            txtRoleName.Text = role.RoleName;
            txtDescription.Text = role.Description;
        }

        private void SaveData()
        {
            if (string.IsNullOrWhiteSpace(txtRoleName.Text))
            {
                MessageBox.Show("Введите название роли", "Ошибка");
                txtRoleName.Focus();
                return;
            }

            role.RoleName = txtRoleName.Text.Trim();
            role.Description = txtDescription.Text.Trim();
        }
    }
}
