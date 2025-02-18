﻿using DDD.Domain.Entities;
using DDD.Domain.Helpers;
using DDD.Domain.Repositories;
using DDD.Domain.ValueObjects;
using DDD.Infrastructure.SQLite;
using DDD.WPF.Services;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;

namespace DDD.WPF.ViewModels
{
    public class WeatherSaveViewModel : ViewModelBase, IDialogAware
    {
        private IWeatherRepository _wather;
        private IAreasRepository _areasRepository;
        private IMessageService _messageService;

        public WeatherSaveViewModel()
            : this(
                  new WeatherSQLite(), 
                  new AreasSQLite(),
                  new MessageService())
        {
        }

        public WeatherSaveViewModel(
            IWeatherRepository wather,
            IAreasRepository areas,
            IMessageService messageService)
        {
            _wather = wather;
            _areasRepository = areas;
            _messageService = messageService;

            DataDateValue = GetDateTime();
            SelectedCondition = Condition.Sunny;
            TemperatureText = string.Empty;

            foreach (var area in _areasRepository.GetData())
            {
                Areas.Add(new AreaEntity(area.AreaId, area.AreaName));
            }

            SaveButton = new DelegateCommand(
                SaveButtonExecute);
        }

        public string Title => "登録画面";

        public event Action<IDialogResult> RequestClose;

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            if(parameters == null)
            {
                return;
            }

            var p = parameters
                .GetValue<WeatherEntity>(nameof(WeatherEntity));
            if (p == null)
            {
                return;
            }

            SelectedArea = Areas.First(x => x.AreaId == p.AreaId.Value);
            DataDateValue = p.DataDate;
            SelectedCondition = Conditions.First(x => x == p.Condition);
            TemperatureText = p.Temperature.Value.ToString();
        }

        private AreaEntity _selectedArea;
        public AreaEntity SelectedArea
        {
            get { return _selectedArea; }
            set
            {
                SetProperty(ref _selectedArea, value);
            }
        }

        private DateTime? _dataDateValue;
        public DateTime? DataDateValue
        {
            get { return _dataDateValue; }
            set
            {
                SetProperty(ref _dataDateValue, value);
            }
        }

        private Condition _selectedCondition;
        public Condition SelectedCondition
        {
            get { return _selectedCondition; }
            set
            {
                SetProperty(ref _selectedCondition, value);
            }
        }

        private string _temperatureUnitName;
        public string TemperatureText
        {
            get { return _temperatureUnitName; }
            set
            {
                SetProperty(ref _temperatureUnitName, value);
            }
        }

        private ObservableCollection<AreaEntity> _areas
             = new ObservableCollection<AreaEntity>();
        public ObservableCollection<AreaEntity> Areas
        {
            get { return _areas; }
            set
            {
                SetProperty(ref _areas, value);
            }
        }

        private ObservableCollection<Condition> _conditions
           = new ObservableCollection<Condition>(Condition.ToList());
        public ObservableCollection<Condition> Conditions
        {
            get { return _conditions; }
            set
            {
                SetProperty(ref _conditions, value);
            }
        }

        public string TemperatureUnitName => Temperature.UnitName;

        public DelegateCommand SaveButton { get; }

        private void SaveButtonExecute()
        {
            Guard.IsNull(SelectedArea, "エリアを選択してください");
            Guard.IsNull(DataDateValue, "日時を入力してください");
            var temperature =
                Guard.IsFloat(TemperatureText, "温度の入力に誤りがあります");

            if (_messageService.Question("保存しますか？") 
                != System.Windows.MessageBoxResult.OK)
            {
                return;
            }

            var entity = new WeatherEntity(
                SelectedArea.AreaId,
                SelectedArea.AreaName,
                DataDateValue.Value,
                SelectedCondition.Value,
                temperature);

            _wather.Save(entity);

            _messageService.ShowDialog("保存しました");
        }
    }
}
