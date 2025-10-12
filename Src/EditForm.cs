using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.Reflection;

namespace SmartphoneDefectsDatabase1
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
            InitializeCustomComponents();
        }

        private void InitializeCustomComponents()
        {
            this.Text = $"Редактирование - {tableName}";
            this.Size = new System.Drawing.Size(800, 350);
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

            tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 200F));
            tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 450F));
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
                Location = new System.Drawing.Point(400, 10),
                Size = new System.Drawing.Size(100, 35),
                Font = new System.Drawing.Font("Microsoft Sans Serif", 10F)
            };

            var btnCancel = new Button
            {
                Text = "Отмена",
                DialogResult = DialogResult.Cancel,
                Location = new System.Drawing.Point(550, 10),
                Size = new System.Drawing.Size(100, 35),
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
                case "Smartphones":
                    return new Smartphone
                    {
                        ProductionDate = DateTime.Today,
                        Model = "Новая модель",
                        Number = "SN000001"
                    };
                case "Screens":
                    return new Screen
                    {
                        Type = "OLED"
                    };
                case "Defects":
                    return new Defect
                    {
                        DetectionDate = DateTime.Now,
                        Type = "Царапина",
                        Severity = "Легкий"
                    };
                case "Parties":
                    return new Party
                    {
                        ArrivalDate = DateTime.Today,
                        PartyName = "Новая партия",
                        Quantity = 1
                    };
                case "Controllers":
                    return new Controller
                    {
                        Name = "Новый контроллер",
                        Login = "user"
                    };
                case "Images":
                    return new Image
                    {
                        FileName = "image.jpg"
                    };
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
                // Создаем панель для каждой пары Label + Control
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
                DateTime dateValue;

                if (currentValue != null && (DateTime)currentValue > DateTimePicker.MinimumDateTime &&
                    (DateTime)currentValue < DateTimePicker.MaximumDateTime)
                {
                    dateValue = (DateTime)currentValue;
                }
                else
                {
                    dateValue = DateTime.Today; // Устанавливаем текущую дату по умолчанию
                }

                return new DateTimePicker
                {
                    Value = dateValue,
                    Format = DateTimePickerFormat.Short,
                    MinDate = new DateTime(2000, 1, 1), // Минимальная дата
                    MaxDate = DateTime.Today.AddYears(1) // Максимальная дата
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

        private void SaveChanges()
        {
            try
            {
                // Собираем значения из контролов
                foreach (Control control in GetAllControls(this))
                {
                    if (control.Tag is PropertyInfo prop)
                    {
                        object value = GetValueFromControl(control, prop.PropertyType);
                        prop.SetValue(entity, value);
                    }
                }

                // Добавляем новую сущность в контекст, если это новая запись
                if (IsNewEntity())
                {
                    AddEntityToContext();
                }
                else
                {
                    dbContext.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                }

                dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.None; // Не закрываем форму при ошибке
            }
        }

        private bool IsNewEntity()
        {
            var idProperty = entity.GetType().GetProperty("ID") ??
                           entity.GetType().GetProperty($"{entity.GetType().Name}ID");

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
                case "Smartphones": dbContext.Smartphones.Add((Smartphone)entity); break;
                case "Screens": dbContext.Screens.Add((Screen)entity); break;
                case "Defects": dbContext.Defects.Add((Defect)entity); break;
                case "Parties": dbContext.Parties.Add((Party)entity); break;
                case "Controllers": dbContext.Controllers.Add((Controller)entity); break;
                case "Images": dbContext.Images.Add((Image)entity); break;
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
                    // Проверяем, что дата в допустимом диапазоне
                    if (dateTimePicker.Value >= dateTimePicker.MinDate && dateTimePicker.Value <= dateTimePicker.MaxDate)
                    {
                        return dateTimePicker.Value;
                    }
                    else
                    {
                        return DateTime.Today; // Возвращаем текущую дату если значение вне диапазона
                    }
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
