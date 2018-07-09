﻿using LiteDB;
using MahApps.Metro.Controls.Dialogs;
using Phony.Kernel;
using Phony.Model;
using Phony.Utility;
using Phony.View;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Phony.ViewModel
{
    public class NoteVM : CommonBase
    {
        long _noId;
        string _name;
        string _phone;
        string _searchText;
        string _notes;
        string _childName;
        string _childNo;
        bool _byName;
        bool _fastResult;
        bool _openFastResult;
        bool _isAddNoFlyoutOpen;
        Note _dataGridSelectedNo;

        ObservableCollection<Note> _numbers;

        public long NoId
        {
            get => _noId;
            set
            {
                if (value != _noId)
                {
                    _noId = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                if (value != _name)
                {
                    _name = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string Phone
        {
            get => _phone;
            set
            {
                if (value != _phone)
                {
                    _phone = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (value != _searchText)
                {
                    _searchText = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string Notes
        {
            get => _notes;
            set
            {
                if (value != _notes)
                {
                    _notes = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string ChildName
        {
            get => _childName;
            set
            {
                if (value != _childName)
                {
                    _childName = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string ChildNo
        {
            get => _childNo;
            set
            {
                if (value != _childNo)
                {
                    _childNo = value;
                    RaisePropertyChanged();
                }
            }
        }

        public bool ByName
        {
            get => _byName;
            set
            {
                if (value != _byName)
                {
                    _byName = value;
                    RaisePropertyChanged();
                }
            }
        }

        public bool FastResult
        {
            get => _fastResult;
            set
            {
                if (value != _fastResult)
                {
                    _fastResult = value;
                    RaisePropertyChanged();
                }
            }
        }

        public bool OpenFastResult
        {
            get => _openFastResult;
            set
            {
                if (value != _openFastResult)
                {
                    _openFastResult = value;
                    RaisePropertyChanged();
                }
            }
        }

        public bool IsAddNoFlyoutOpen
        {
            get => _isAddNoFlyoutOpen;
            set
            {
                if (value != _isAddNoFlyoutOpen)
                {
                    _isAddNoFlyoutOpen = value;
                    RaisePropertyChanged();
                }
            }
        }

        public Note DataGridSelectedNo
        {
            get => _dataGridSelectedNo;
            set
            {
                if (value != _dataGridSelectedNo)
                {
                    _dataGridSelectedNo = value;
                    RaisePropertyChanged();
                }
            }
        }

        public ObservableCollection<Note> Numbers
        {
            get => _numbers;
            set
            {
                if (value != _numbers)
                {
                    _numbers = value;
                    RaisePropertyChanged();
                }
            }
        }

        public ObservableCollection<User> Users { get; set; }

        public ICommand AddNo { get; set; }
        public ICommand EditNo { get; set; }
        public ICommand DeleteNo { get; set; }
        public ICommand OpenAddNoFlyout { get; set; }
        public ICommand SelectImage { get; set; }
        public ICommand FillUI { get; set; }
        public ICommand ReloadAllNos { get; set; }
        public ICommand Search { get; set; }

        Users.LoginVM CurrentUser = new Users.LoginVM();

        Notes Message = Application.Current.Windows.OfType<Notes>().FirstOrDefault();

        public NoteVM()
        {
            LoadCommands();
            ByName = true;
            using (var db = new LiteDatabase(Properties.Settings.Default.DBFullName))
            {
                Numbers = new ObservableCollection<Note>(db.GetCollection<Note>(DBCollections.Notes.ToString()).FindAll());
                Users = new ObservableCollection<User>(db.GetCollection<User>(DBCollections.Users.ToString()).FindAll());
            }
        }

        public void LoadCommands()
        {
            AddNo = new CustomCommand(DoAddNo, CanAddNo);
            EditNo = new CustomCommand(DoEditNo, CanEditNo);
            DeleteNo = new CustomCommand(DoDeleteNo, CanDeleteNo);
            OpenAddNoFlyout = new CustomCommand(DoOpenAddNoFlyout, CanOpenAddNoFlyout);
            FillUI = new CustomCommand(DoFillUI, CanFillUI);
            ReloadAllNos = new CustomCommand(DoReloadAllNos, CanReloadAllNos);
            Search = new CustomCommand(DoSearch, CanSearch);
        }

        private bool CanAddNo(object obj)
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                return false;
            }
            return true;
        }

        private void DoAddNo(object obj)
        {
            using (var db = new LiteDatabase(Properties.Settings.Default.DBFullName))
            {
                var n = new Note
                {
                    Name = Name,
                    Phone = Phone,
                    Group = NoteGroup.Numbers,
                    Notes = Notes,
                    CreateDate = DateTime.Now,
                    Creator = db.GetCollection<User>(DBCollections.Users.ToString()).FindById(CurrentUser.Id),
                    EditDate = null,
                    Editor = null
                };
                db.GetCollection<Note>(DBCollections.Notes.ToString()).Insert(n);
                Numbers.Add(n);
                Message.ShowMessageAsync("تمت العملية", "تم اضافة الرقم بنجاح");
            }
        }

        private bool CanEditNo(object obj)
        {
            if (string.IsNullOrWhiteSpace(Name) || NoId == 0 || DataGridSelectedNo == null)
            {
                return false;
            }
            return true;
        }

        private void DoEditNo(object obj)
        {
            using (var db = new LiteDatabase(Properties.Settings.Default.DBFullName))
            {
                var n = db.GetCollection<Note>(DBCollections.Notes.ToString()).FindById(DataGridSelectedNo.Id);
                n.Name = Name;
                n.Phone = Phone;
                n.Notes = Notes;
                db.GetCollection<Note>(DBCollections.Notes.ToString()).Update(n);
                Numbers[Numbers.IndexOf(DataGridSelectedNo)] = n;
                NoId = 0;
                DataGridSelectedNo = null;
                Message.ShowMessageAsync("تمت العملية", "تم تعديل الرقم بنجاح");
            }
        }

        private bool CanDeleteNo(object obj)
        {
            if (DataGridSelectedNo == null)
            {
                return false;
            }
            return true;
        }

        private async void DoDeleteNo(object obj)
        {
            var result = await Message.ShowMessageAsync("حذف الرقم", $"هل انت متاكد من حذف الرقم {DataGridSelectedNo.Name}", MessageDialogStyle.AffirmativeAndNegative);
            if (result == MessageDialogResult.Affirmative)
            {
                using (var db = new LiteDatabase(Properties.Settings.Default.DBFullName))
                {
                    db.GetCollection<Note>(DBCollections.Notes.ToString()).Delete(DataGridSelectedNo.Id);
                    Numbers.Remove(DataGridSelectedNo);
                }
                DataGridSelectedNo = null;
                await Message.ShowMessageAsync("تمت العملية", "تم حذف الرقم بنجاح");
            }
        }

        private bool CanSearch(object obj)
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                return false;
            }
            return true;
        }

        private void DoSearch(object obj)
        {
            try
            {
                using (var db = new LiteDatabase(Properties.Settings.Default.DBFullName))
                {
                    Numbers = new ObservableCollection<Note>(db.GetCollection<Note>(DBCollections.Notes.ToString()).Find(i => i.Name.Contains(SearchText)));
                    if (Numbers.Count > 0)
                    {
                        if (FastResult)
                        {
                            ChildName = Numbers.FirstOrDefault().Name;
                            ChildNo = Numbers.FirstOrDefault().Phone;
                            OpenFastResult = true;
                        }
                    }
                    else
                    {
                        Message.ShowMessageAsync("غير موجود", "لم يتم العثور على شئ");
                    }
                }
            }
            catch (Exception ex)
            {
                Core.SaveException(ex);
                BespokeFusion.MaterialMessageBox.ShowError("لم يستطع ايجاد ما تبحث عنه تاكد من صحه البيانات المدخله");
            }
        }

        private bool CanReloadAllNos(object obj)
        {
            return true;
        }

        private void DoReloadAllNos(object obj)
        {
            using (var db = new LiteDatabase(Properties.Settings.Default.DBFullName))
            {
                Numbers = new ObservableCollection<Note>(db.GetCollection<Note>(DBCollections.Notes.ToString()).FindAll());
            }
        }

        private bool CanFillUI(object obj)
        {
            if (DataGridSelectedNo == null)
            {
                return false;
            }
            return true;
        }

        private void DoFillUI(object obj)
        {
            NoId = DataGridSelectedNo.Id;
            Name = DataGridSelectedNo.Name;
            Phone = DataGridSelectedNo.Phone;
            Notes = DataGridSelectedNo.Notes;
            IsAddNoFlyoutOpen = true;
        }

        private bool CanOpenAddNoFlyout(object obj)
        {
            return true;
        }

        private void DoOpenAddNoFlyout(object obj)
        {
            if (IsAddNoFlyoutOpen)
            {
                IsAddNoFlyoutOpen = false;
            }
            else
            {
                IsAddNoFlyoutOpen = true;
            }
        }
    }
}