﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Geeks.VSIX.TidyCSharp.Cleanup;
using Geeks.VSIX.TidyCSharp.Cleanup.Infra;
using Geeks.VSIX.TidyCSharp.Menus.Cleanup.CommandsHandlers.Infra;

namespace Geeks.VSIX.TidyCSharp.Menus.Cleanup.CommandsHandlers
{
    public partial class CleanupItemUserControl : UserControl, IMainCleanup
    {
        public CleanupItemUserControl()
        {
            InitializeComponent();
            customCheckListBox1.BorderStyle = BorderStyle.None;
            checkboxCleanupItem.CheckedChanged += CheckboxCleanupItem_CheckedChanged;
            customCheckListBox1.Enabled = checkboxCleanupItem.Checked;
        }

        public void Init(CodeCleanerType mainCleanupItemType)
        {
            MainCleanupItemType = mainCleanupItemType;

            var values = Get(typeof(CodeCleanerType), mainCleanupItemType);

            checkboxCleanupItem.Text =
                values.Value != null ?
                    values.Value.Title ?? mainCleanupItemType.ToString() :
                    mainCleanupItemType.ToString();

            if (values.Value.SubItemType != null)
            {
                HasSubItems = true;

                CreateControls(values.Value.SubItemType, checkBoxItem => NewCheckboxItem(checkBoxItem));
                this.BorderStyle = BorderStyle.FixedSingle;

                this.Height += customCheckListBox1.Controls.Count * customCheckListBox1.Controls[0].Height;
                return;
            }

            this.Height = CustomCheckListBox.HEIGHT_OF_CHECKBOX + 5;

        }

        #region IMainCleanup

        public CodeCleanerType MainCleanupItemType { get; private set; }
        public bool HasSubItems { get; private set; } = false;

        public void SetSubItems(int value)
        {
            if (value == -1)
            {
                checkboxCleanupItem.Checked = true;
                return;
            }
            customCheckListBox1.SetCheckedItems(value);
            checkboxCleanupItem.Checked = customCheckListBox1.GetCheckedItems().Any();
        }

        public void ReSetSubItems(bool selectAll = false)
        {
            checkboxCleanupItem.Checked = selectAll;

            customCheckListBox1.ReSetSubItems(selectAll);
        }

        public bool IsMainObjectSelected
        {
            get
            {
                if (checkboxCleanupItem.Checked == false) return false;
                if (HasSubItems && GetSelectedSubItems().Any() == false) return false;
                return true;
            }
        }

        public CheckBoxItemInfo[] GetSelectedSubItems()
        {
            if (checkboxCleanupItem.Checked == false) return new CheckBoxItemInfo[0];

            var selectedTypes = customCheckListBox1.GetCheckedItems();
            return selectedTypes.OrderBy(x => x.Order).ToArray();
        }

        //public CheckBoxItemInfo[] GetSubItems()
        //{
        //    return customCheckListBox1.GetItems();
        //}

        #endregion

        public static void CreateControls(Type subItemType, Action<CheckBoxItemInfo> action, bool sortDESC = false)
        {
            try
            {
                var items = Get(subItemType);

                if (!sortDESC) items = items.OrderBy(x => x.Value.Order.GetValueOrDefault(x.Key));
                else items = items.OrderByDescending(x => x.Value.Order.GetValueOrDefault(x.Key));

                foreach (var item in items)
                {
                    var tempCheckBoxItem = new CheckBoxItemInfo() { CleanerType = item.Key };

                    tempCheckBoxItem.Order = item.Value.Order.GetValueOrDefault(item.Key);

                    if (!string.IsNullOrEmpty(item.Value.Title))
                    {
                        tempCheckBoxItem.Name = item.Value.Title;
                    }
                    else
                    {
                        tempCheckBoxItem.Name = Enum.GetName(subItemType, item.Key).ToString();
                    }
                    action(tempCheckBoxItem);
                }
            }
            catch (System.Exception exp)
            {

            }
        }

        static IEnumerable<KeyValuePair<int, CleanupItemAttribute>> Get(Type subItemType)
        {
            foreach (var item in Enum.GetValues(subItemType))
            {
                yield return Get(subItemType, item);
            }
        }

        static KeyValuePair<int, CleanupItemAttribute> Get(Type subItemType, object item)
        {
            var memInfo = subItemType.GetMember(item.ToString());
            var attributes = memInfo[0].GetCustomAttributes(typeof(CleanupItemAttribute), false);
            return new KeyValuePair<int, CleanupItemAttribute>((int)item,
                (CleanupItemAttribute)attributes.FirstOrDefault()
                ??
                new CleanupItemAttribute()
                );
        }

        void NewCheckboxItem(CheckBoxItemInfo checkBoxItem)
        {
            customCheckListBox1.AddItem(checkBoxItem);
        }
        void CheckboxCleanupItem_CheckedChanged(object sender, EventArgs e)
        {
            customCheckListBox1.Enabled = checkboxCleanupItem.Checked;
        }

    }
}