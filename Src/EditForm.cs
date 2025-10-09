using System;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms;

namespace SmartphoneDefectsDatabase
{
    public partial class EditForm : Form
    {
        private DefectContext dbContext;
        private string tableName;
        private object entity;
        
        public EditForm(DefectContext context, string table, object item = null)
        {
            dbContext = context;
            tableName = table;
            entity = item ?? CreateNewEntity();
            InitializeCustomComponents(); // Изменили название метода
        }

        private void InitializeCustomComponents()
        {
            this.Text = $"Редактирование - {tableName}";
            this.Size = new System.Drawing.Size(800, 600);
            this.StartPosition = FormStartPosition.CenterParent;

            var properties = entity.GetType().GetProperties()
                .Where(p => p.CanWrite &&
                           !p.Name.EndsWith("ID") &&
                           p.Name != "ID" &&
                           p.Name != "Smartphone" &&
                           p.Name != "Screen" &&
                           p.Name != "Defect")
                .ToList();

            var tableLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = properties.Count + 1,
                Padding = new Padding(20),
                AutoScroll = true
            };

            tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 200F)); // Метки
            tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 450F)); // Поля ввода

            for (int i = 0; i < properties.Count; i++)
            {
                var prop = properties[i];
                tableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 60F));

                var label = new Label
                {
                    Text = GetDisplayName(prop.Name),
                    Dock = DockStyle.Fill,
                    Font = new System.Drawing.Font("Microsoft Sans Serif", 10F),
                    TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
                    Height = 25
                };

                Control inputControl = CreateInputControl(prop);
                inputControl.Dock = DockStyle.Fill;
                inputControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
                inputControl.Tag = prop;

                tableLayout.Controls.Add(label, 0, i);
                tableLayout.Controls.Add(inputControl, 1, i);
            }

            tableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 70F));

            var buttonPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Height = 50
            };

            var btnOK = new Button
            {
                Text = "OK",
                DialogResult = DialogResult.OK,
                Location = new System.Drawing.Point(500, 10),
                Size = new System.Drawing.Size(95, 35),
                Font = new System.Drawing.Font("Microsoft Sans Serif", 10F)
            };

            var btnCancel = new Button
            {
                Text = "Отмена",
                DialogResult = DialogResult.Cancel,
                Location = new System.Drawing.Point(610, 10),
                Size = new System.Drawing.Size(95, 35),
                Font = new System.Drawing.Font("Microsoft Sans Serif", 10F)
            };

            btnOK.Click += (s, e) => { SaveChanges(); };

            buttonPanel.Controls.Add(btnOK);
            buttonPanel.Controls.Add(btnCancel);

            tableLayout.Controls.Add(buttonPanel, 0, properties.Count);
            tableLayout.SetColumnSpan(buttonPanel, 2);

            this.Controls.Add(tableLayout);
            this.AcceptButton = btnOK;
            this.CancelButton = btnCancel;
        }

        private object CreateNewEntity()
        {
            switch (tableName)
            {
                case "Parties":
                    return new Party { Number = "P" + DateTime.Now.ToString("yyyy-MM-001"), Quantity = 1 };
                case "Smartphones":
                    return new Smartphone { Model = "Новая модель", Number = "SN" + DateTime.Now.ToString("yyyyMMdd-001") };
                case "Controllers":  // Используем Controllers вместо Operators
                    return new Controller { Name = "Имя", Surname = "Фамилия" };
                case "Screens":
                    return new Screen { Material = "Стекло", Supplier = "Поставщик" };
                case "Defects":
                    return new Defect { Type = "Царапина", Size = 0.1m, Coordinates = "X:0,Y:0" };
                case "Images":
                    return new Image { FilePath = "C:\\Images\\" };
                case "Roles":
                    return new Role { RoleName = "Новая роль" };
                case "Users":
                    return new User { Username = "newuser", FullName = "Новый пользователь", Password = "password" };
                default:
                    throw new ArgumentException($"Неизвестная таблица: {tableName}");
            }
        }

        private void CreateInputFields(FlowLayoutPanel panel)
        {
            var properties = entity.GetType().GetProperties()
                .Where(p => p.CanWrite && 
                           !p.Name.EndsWith("ID") && 
                           p.Name != "ID" &&
                           p.Name != "Smartphone" && 
                           p.Name != "Screen" && 
                           p.Name != "Defect")
                .ToList();
                
            foreach (var prop in properties)
            {
                var fieldPanel = new Panel 
                { 
                    Width = panel.Width - 25, 
                    Height = 60,
                    Margin = new Padding(0, 5, 0, 5)
                };
                
                var label = new Label 
                { 
                    Text = GetDisplayName(prop.Name),
                    Location = new System.Drawing.Point(0, 5),
                    Width = 150,
                    Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular)
                };
                
                Control inputControl = CreateInputControl(prop);
                inputControl.Location = new System.Drawing.Point(160, 3);
                inputControl.Width = 200;
                inputControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular);
                inputControl.Tag = prop;
                
                fieldPanel.Controls.Add(label);
                fieldPanel.Controls.Add(inputControl);
                
                panel.Controls.Add(fieldPanel);
            }
        }

        private Control CreateInputControl(PropertyInfo prop)
        {
            var currentValue = prop.GetValue(entity);
            
            if (prop.PropertyType == typeof(DateTime))
            {
                return new DateTimePicker 
                { 
                    Value = currentValue != null ? (DateTime)currentValue : DateTime.Now,
                    Format = DateTimePickerFormat.Short
                };
            }
            else if (prop.PropertyType == typeof(bool))
            {
                var checkBox = new CheckBox { Checked = currentValue != null && (bool)currentValue };
                checkBox.Height = 20;
                return checkBox;
            }
            else if (prop.PropertyType == typeof(decimal) || prop.PropertyType == typeof(int))
            {
                return new NumericUpDown 
                { 
                    Value = currentValue != null ? Convert.ToDecimal(currentValue) : 0,
                    DecimalPlaces = prop.PropertyType == typeof(decimal) ? 2 : 0
                };
            }
            else
            {
                return new TextBox { Text = currentValue?.ToString() ?? "" };
            }
        }

        private string GetDisplayName(string propertyName)
        {
            var displayNames = new System.Collections.Generic.Dictionary<string, string>
            {
                { "Model", "Модель" },
                { "SerialNumber", "Серийный номер" },
                { "ProductionDate", "Дата производства" },
                { "ScreenType", "Тип экрана" },
                { "Size", "Размер" },
                { "DefectType", "Тип дефекта" },
                { "Severity", "Серьезность" },
                { "Location", "Расположение" },
                { "DetectionDate", "Дата обнаружения" },
                { "PartyName", "Название партии" },
                { "ArrivalDate", "Дата поступления" },
                { "Quantity", "Количество" },
                { "Name", "Имя" },
                { "Login", "Логин" },
                { "FileName", "Имя файла" }
            };
            
            return displayNames.ContainsKey(propertyName) ? displayNames[propertyName] : propertyName;
        }
        private bool ValidateEntity()
        {
            var validationErrors = new List<string>();

            if (entity is Smartphone smartphone)
            {
                if (string.IsNullOrWhiteSpace(smartphone.Model))
                    validationErrors.Add("Модель: Обязательное поле");
                if (string.IsNullOrWhiteSpace(smartphone.Manufacturer))
                    validationErrors.Add("Производитель: Обязательное поле");
                if (string.IsNullOrWhiteSpace(smartphone.Number))
                    validationErrors.Add("Номер: Обязательное поле");
                if (smartphone.PartyID == 0)
                    validationErrors.Add("Партия: Необходимо выбрать партию");
            }
            else if (entity is Party party)
            {
                if (string.IsNullOrWhiteSpace(party.Number))
                    validationErrors.Add("Номер партии: Обязательное поле");
                if (party.Quantity <= 0)
                    validationErrors.Add("Количество: Должно быть больше 0");
            }
            else if (entity is Controller controller)
            {
                if (string.IsNullOrWhiteSpace(controller.Name))
                    validationErrors.Add("Имя: Обязательное поле");
                if (string.IsNullOrWhiteSpace(controller.Surname))
                    validationErrors.Add("Фамилия: Обязательное поле");
            }
            else if (entity is Screen screen)
            {
                if (string.IsNullOrWhiteSpace(screen.Material))
                    validationErrors.Add("Материал: Обязательное поле");
                if (string.IsNullOrWhiteSpace(screen.Supplier))
                    validationErrors.Add("Поставщик: Обязательное поле");
                if (screen.SmartphoneID == 0)
                    validationErrors.Add("Смартфон: Необходимо выбрать смартфон");
            }
            else if (entity is Defect defect)
            {
                if (string.IsNullOrWhiteSpace(defect.Type))
                    validationErrors.Add("Тип дефекта: Обязательное поле");
                if (defect.Size <= 0)
                    validationErrors.Add("Размер: Должен быть больше 0");
                if (string.IsNullOrWhiteSpace(defect.Coordinates))
                    validationErrors.Add("Координаты: Обязательное поле");
                if (defect.ScreenID == 0)
                    validationErrors.Add("Экран: Необходимо выбрать экран");
                if (defect.ControllerID == 0)
                    validationErrors.Add("Контроллер: Необходимо выбрать контроллера");
            }
            else if (entity is Image image)
            {
                if (string.IsNullOrWhiteSpace(image.FilePath))
                    validationErrors.Add("Путь к файлу: Обязательное поле");
                if (image.DefectID == 0)
                    validationErrors.Add("Дефект: Необходимо выбрать дефект");
            }
            else if (entity is Role role)
            {
                if (string.IsNullOrWhiteSpace(role.RoleName))
                    validationErrors.Add("Название роли: Обязательное поле");
            }
            else if (entity is User user)
            {
                if (string.IsNullOrWhiteSpace(user.Username))
                    validationErrors.Add("Логин: Обязательное поле");
                if (string.IsNullOrWhiteSpace(user.Password))
                    validationErrors.Add("Пароль: Обязательное поле");
                if (string.IsNullOrWhiteSpace(user.FullName))
                    validationErrors.Add("ФИО: Обязательное поле");
                if (user.RoleID == 0)
                    validationErrors.Add("Роль: Необходимо выбрать роль");
            }

            if (validationErrors.Any())
            {
                string errorMessage = "Обнаружены ошибки:\n" + string.Join("\n", validationErrors);
                MessageBox.Show(errorMessage, "Ошибки валидации", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }
        private void SaveChanges()
        {
            try
            {

                foreach (Control control in GetAllControls(this))
                {
                    if (control.Tag is PropertyInfo prop)
                    {
                        object value = GetValueFromControl(control, prop.PropertyType);
                        prop.SetValue(entity, value);
                    }
                }

                if (!ValidateEntity())
                {
                    this.DialogResult = DialogResult.None;
                    return;
                }

                if (IsNewEntity())
                {
                    AddEntityToContext();
                }
                else
                {
                    dbContext.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                }
                dbContext.SaveChanges();
                MessageBox.Show("Данные успешно сохранены!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                // Обработка ошибок валидации
                string errorMessage = "Ошибки валидации:\n";
                foreach (var validationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        errorMessage += $"- {validationError.PropertyName}: {validationError.ErrorMessage}\n";
                    }
                }
                MessageBox.Show(errorMessage, "Ошибка валидации", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.None;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}\n\nТип ошибки: {ex.GetType().Name}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.None;
            }
        }

        private bool IsNewEntity()
        {
            var idProperty = entity.GetType().GetProperty("ID") ??
                  entity.GetType().GetProperty($"{entity.GetType().Name}ID") ??
                  entity.GetType().GetProperties()
                        .FirstOrDefault(p => p.Name.EndsWith("ID") && p.PropertyType == typeof(int));

            if (idProperty != null)
            {
                var idValue = idProperty.GetValue(entity);
                return idValue == null || (idValue is int && (int)idValue == 0);
            }

            return true;
        }

        private void AddEntityToContext()
        {
            switch (tableName)
            {
                case "Parties":
                    dbContext.Parties.Add((Party)entity);
                    break;
                case "Smartphones":
                    dbContext.Smartphones.Add((Smartphone)entity);
                    break;
                case "Controllers":
                    dbContext.Controllers.Add((Controller)entity);
                    break;
                case "Screens":
                    dbContext.Screens.Add((Screen)entity);
                    break;
                case "Defects":
                    dbContext.Defects.Add((Defect)entity);
                    break;
                case "Images":
                    dbContext.Images.Add((Image)entity);
                    break;
                case "Roles":
                    dbContext.Roles.Add((Role)entity);
                    break;
                case "Users":
                    dbContext.Users.Add((User)entity);
                    break;
                default:
                    throw new ArgumentException($"Неизвестная таблица: {tableName}");
            }
        }

        private object GetValueFromControl(Control control, Type targetType)
        {
            try
            {
                if (control is TextBox textBox)
                {
                    if (targetType == typeof(string)) return textBox.Text;
                    if (targetType == typeof(int) && int.TryParse(textBox.Text, out int intValue)) return intValue;
                    if (targetType == typeof(decimal) && decimal.TryParse(textBox.Text, out decimal decimalValue)) return decimalValue;
                    return Convert.ChangeType(textBox.Text, targetType);
                }
                else if (control is DateTimePicker dateTimePicker)
                {
                    return dateTimePicker.Value;
                }
                else if (control is CheckBox checkBox)
                {
                    return checkBox.Checked;
                }
                else if (control is NumericUpDown numericUpDown)
                {
                    return Convert.ChangeType(numericUpDown.Value, targetType);
                }
                
                return null;
            }
            catch
            {
                return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;
            }
        }

        private System.Collections.Generic.IEnumerable<Control> GetAllControls(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                yield return control;
                foreach (Control childControl in GetAllControls(control))
                    yield return childControl;
            }
        }
    }
}
