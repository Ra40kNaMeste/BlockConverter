using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using ModelConverterToBlock.Blocks;
using BlockConverter.BlockFactorys;
using BlocksLib.Blocks;
using BlocksLib.Blocks.Tools;
using System.Windows.Media.Imaging;
using BlockConverter.Tools;
using Microsoft.Win32;
using System.IO;
using System.Reflection;
using ModelConverterToBlock.Converters.ConvertToBlock;
using ModelConverterToBlock.Converters.Reader;
using System.Drawing;
using System.Windows.Media;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using BlockConverter.ViewModels.SettingViewModel;
using System.Windows.Input;
using System.Threading;
using ModelConverterToBlock.Logger;
using System.Windows.Documents;

namespace BlockConverter.ViewModels
{
    class RootViewModel : INotifyPropertyChanged
    {
        #region Commands

        #region HeadCommand
        private CommandWithException addBlock;
        public CommandWithException AddBlock
        {
            get
            {
                return addBlock ??= new CommandWithException((object val) => InvokeTargetCommand(SelectScheme.AddBlockCommand, val),
                    () => SetDialog(Resources.Resources.AddedSuccessfullyBlock),
                    (Exception ex) => SetDialogError(Resources.Resources.AddedFailBlock + ex.Message),
                    (object val) => CanInvokeTargetCommand(SelectScheme?.AddBlockCommand, val));
            }
        }

        private CommandWithException removeBlock;
        public CommandWithException RemoveBlock
        {
            get
            {
                return removeBlock ??= new CommandWithException((object val) => InvokeTargetCommand(SelectScheme.RemoveBlockCommand, val),
                    () => SetDialog(Resources.Resources.RemoveSuccessfullyBlock),
                    (Exception ex) => SetDialogError(Resources.Resources.RemoveFailBlock + ex.Message),
                    (object val) => CanInvokeTargetCommand(SelectScheme?.RemoveBlockCommand, val));
            }
        }

        private CommandWithException undo;
        public CommandWithException Undo
        {
            get
            {
                return undo ??= new CommandWithException((object val) => InvokeTargetCommand(SelectScheme.UndoCommand, val),
                    () => SetDialog(Resources.Resources.UndoSuccessfully),
                    (Exception ex) => SetDialogError(Resources.Resources.UndoFailed + ex.Message),
                    (object val) => CanInvokeTargetCommand(SelectScheme?.UndoCommand, val));
            }
        }

        private CommandWithException redo;
        public CommandWithException Redo
        {
            get
            {
                return redo ??= new CommandWithException((object val) => InvokeTargetCommand(SelectScheme.RedoCommand, val),
                    () => SetDialog(Resources.Resources.RedoSuccessfully),
                    (Exception ex) => SetDialogError(Resources.Resources.RedoFailed + ex.Message),
                    (object val) => CanInvokeTargetCommand(SelectScheme?.RedoCommand, val));
            }
        }

        private CommandWithException copyBlocks;
        public CommandWithException CopyBlocks
        {
            get
            {
                return copyBlocks ??= new CommandWithException((object val) => InvokeTargetCommand(SelectScheme.CopyCommand, val),
                    () => SetDialog(Resources.Resources.CopyBlocksSuccessfully),
                    (Exception ex) => SetDialogError(Resources.Resources.CopyBlocksFailed + ex.Message),
                    (object val) => CanInvokeTargetCommand(SelectScheme?.CopyCommand, val));
            }
        }

        private CommandWithException pasteBlocks;
        public CommandWithException PasteBlocks
        {
            get
            {
                return pasteBlocks ??= new CommandWithException((object val) => InvokeTargetCommand(SelectScheme.PasteCommand, val),
                    () => SetDialog(Resources.Resources.PasteBlocksSuccessfully),
                    (Exception ex) => SetDialogError(Resources.Resources.PasteBlocksFailed + ex.Message),
                    (object val) => CanInvokeTargetCommand(SelectScheme?.PasteCommand, val));
            }
        }

        private CommandWithException export;
        public CommandWithException Export
        {
            get
            {
                return export ??= new CommandWithException(ExportCommand,
                    () => SetDialog(Resources.Resources.ExportBlocksSuccessfully),
                    (Exception ex) => SetDialogError(Resources.Resources.ExportBlocksFailed + ex.Message), CanExportCommand);
            }
        }

        private CommandWithException save;
        public CommandWithException Save
        {
            get
            {
                return save ??= new CommandWithException(SaveCommand,
                    () => SetDialog(Resources.Resources.SaveBlockSchemeSuccessfully),
                    (Exception ex) => SetDialogError(Resources.Resources.SaveBlockSchemeFailed + ex.Message));
            }
        }

        private CommandWithException open;
        public CommandWithException Open
        {
            get
            {
                return open ??= new CommandWithException(OpenCommand,
                    () => SetDialog(Resources.Resources.OpenBlockSchemeSuccessfully),
                    (Exception ex) => SetDialogError(Resources.Resources.OpenBlockSchemeFailed + ex.Message));
            }
        }

        private CommandWithException convert;
        public CommandWithException Convert
        {
            get
            {
                return convert ??= new CommandWithException(ConvertCommand,
                () => SetDialog(Resources.Resources.ConvertStringToBlocksSuccessfully),
                (Exception ex) => SetDialogError(Resources.Resources.ConvertStringToBlocksFailed + ex.Message), CanConvertCommand);
            }
        }

        private CommandWithException readLaguange;
        public CommandWithException ReadLaguange
        {
            get
            {
                return readLaguange ??= new(ReadLaguangeCommand,
                () => SetDialog(Resources.Resources.ReadLaguageSuccessfully),
                (Exception ex) => SetDialogError(Resources.Resources.ReadLaguageFailed + ex.Message), CanConvertCommand);
            }
        }

        private CommandWithException addScheme;
        public CommandWithException AddScheme
        {
            get
            {
                return addScheme ??= new(AddSchemeCommand,
                () => SetDialog(Resources.Resources.AddSchemeSuccessfully),
                (Exception ex) => SetDialogError(Resources.Resources.AddSchemeFailed + ex.Message), CanConvertCommand);
            }
        }

        private CommandWithException removeScheme;
        public CommandWithException RemoveScheme
        {
            get
            {
                return removeScheme ??= new(RemoveSchemeCommand,
                () => SetDialog(Resources.Resources.RemoveSchemeSuccessfully),
                (Exception ex) => SetDialogError(Resources.Resources.RemoveSchemeFailed + ex.Message), CanConvertCommand);
            }
        }

        private CommandWithException openSettings;
        public CommandWithException OpenSettings
        {
            get
            {
                return openSettings ??= new(OpenSettingsCommand);
            }
        }
        #endregion//HeadCommand

        #region BodyCommand


        public bool CanExportCommand(object parameter) => !(parameter == null && ConvertToBitmapEncoder(parameter.ToString()) == null);

        public void ExportCommand(object parameter)
        {
            BitmapEncoder encoder = ConvertToBitmapEncoder(parameter.ToString());
            SelectScheme.SelectDictionary.ClearSelects();
            RenderTargetBitmap bitmap = BitmapConverter.GetRenderTargetBitMaps(SelectScheme.RootPanel);
            encoder.Frames.Add(BitmapFrame.Create(bitmap));
            SaveFileDialog ofd = new SaveFileDialog()
            {
                Title = "Export to...",
                FileName = "BlocksImage",
                DefaultExt = "*.png",
                Filter = "PNG|.png"
            };
            ofd.Title = "Export to";
            if (ofd.ShowDialog() == true)
            {
                string path = ofd.FileName;
                using (FileStream fs = new(path, FileMode.Create))
                {
                    encoder.Save(fs);
                }

            }

        }

        public void SaveCommand(object parameter)
        {
            SaveFileDialog dialog = new SaveFileDialog()
            {
                Title = "Save as",
                DefaultExt = "*.blocks",
                FileName = "MyBlocks",
                Filter = "Block Files|*.blocks"
            };
            if (dialog.ShowDialog() == true)
                BlockComponentOperations.Save(dialog.FileName, SelectScheme.RootComponent);
        }

        public void OpenCommand(object parameter)
        {
            OpenFileDialog dialog = new OpenFileDialog() { Filter = "All files (*" + BlockComponentOperations.format + ")|*" + BlockComponentOperations.format, Title = "Open..." };
            if (dialog.ShowDialog() == true)
            {
                Schemes.Add(new BlockSchemeFromViewItem() { RootComponent = (BeginBlockComponent)BlockComponentOperations.Read(dialog.FileName) });
            }
        }

        public bool CanConvertCommand(object parameter) => converter.IsRead;

        public void ConvertCommand(object parameter)
        {
            var component = converter.Convert((string)parameter);
            foreach (var item in component)
            {
                Schemes.Add(new BlockSchemeFromViewItem() { RootComponent = item });
            }
        }

        public void ReadLaguangeCommand(object parameter)
        {
            SelectionChangedEventArgs e = (SelectionChangedEventArgs)parameter;
            converter.ReadAsync(e.AddedItems[0].ToString());
            SelectInstruction = (FileInfo)e.AddedItems[0];
        }

        private void AddSchemeCommand(object parameter)
        {
            BeginBlockComponent block = new BeginBlockComponent() { Content = "Begin", Name = "New Scheme" };
            block.Add(new BlockEndComponent() { Content = "End" }, new BlockComponentMetadata());
            var res = new BlockSchemeFromViewItem() { RootComponent = block };
            Schemes.Add(res);
            SelectScheme = res;
        }

        private void RemoveSchemeCommand(object parameter)
        {

            if (parameter is BlockSchemeFromViewItem item)
                Schemes.Remove(item);
            else
                Schemes.Remove(SelectScheme);
        }

        private bool CanInvokeTargetCommand(ICommand targetCommand, object parameter) => targetCommand?.CanExecute(parameter) ?? false;
        private void InvokeTargetCommand(ICommand targetCommand, object parameter)
        {
            if (targetCommand?.CanExecute(parameter) ?? false)
                targetCommand.Execute(parameter);
        }
        //передать окно
        public void OpenSettingsCommand(object parameter)
        {
            if (parameter is Window window)
            {
                var ownerWindows = window.OwnedWindows;
                foreach (var item in ownerWindows)
                    if (item is SettingsWindow setting)
                    {
                        setting.Topmost = true;
                        return;
                    }
                SettingsWindow settings = new();
                settings.Owner = window;
                settings.Show();
            }
            else
                throw new ArgumentException("Ожидалось окно");

        }
        #endregion//BodyCommand

        #endregion//Commands

        #region VisualProperties
        public List<FileInfo> Instructions
        {
            get { return PatternLaguangeList.Instructions; }
        }
        private FileInfo selectInstruction;
        public FileInfo SelectInstruction
        {
            get { return selectInstruction; }
            set { selectInstruction = value; OnPropertyChanged(); }
        }
        private string dialog;
        public string Dialog
        {
            get { return dialog; }
            set { dialog = value; OnPropertyChanged(); }
        }
        private SolidColorBrush dialogColor;
        public SolidColorBrush DialogColor
        {
            get { return dialogColor; }
            set { dialogColor = value; OnPropertyChanged(); }
        }

        public IEnumerable<BlockBase> InputBlocks { get; set; }

        //панель, на которой находятся блоки
        private ObservableCollection<BlockSchemeFromViewItem> schemes;
        public ObservableCollection<BlockSchemeFromViewItem> Schemes
        {
            get { return schemes; }
            set
            {
                schemes = value;
                OnPropertyChanged();
            }
        }

        private BlockSchemeFromViewItem selectScheme;
        public BlockSchemeFromViewItem SelectScheme
        {
            get { return selectScheme; }
            set
            {
                selectScheme = value;
                OnPropertyChanged();
            }
        }

        public ISelectDictionaryMode SelectMode { get; set; }
        public IEnumerable<ISelectDictionaryMode> SelectModes { get; set; }

        #endregion

        #region Constructions

        static RootViewModel()
        {
            ConverterSetting.OpenPropertyConverters();
        }

        public RootViewModel()
        {
            Initialize();
        }

        private void Initialize()
        {
            Schemes = new();
            var itemsView = (IEditableCollectionView)CollectionViewSource.GetDefaultView(Schemes);
            itemsView.NewItemPlaceholderPosition = NewItemPlaceholderPosition.AtEnd;

            Settings.Default.Reload();
            SettingViewModel.SettingViewModel.SetSettings();
            FillInputBlocks();
            ReadInstructionsLanguange();
            LogLevels level;
#if DEBUG
            level = LogLevels.Debug;
#else
            level = LogLevels.Info;
#endif
            MemoryLogger = new() { Level = LogLevels.Debug };
            Logger = new() { Level = level };
            Logger.Handler += SetDialog;
            BlockLoggerManager.AddLogger(MemoryLogger);
            BlockLoggerManager.AddLogger(Logger);
            CreateStandartScheme();
            Logger.Log(Resources.Resources.SuccessorfullyInizializeRootViewModel, LogLevels.Info, Thread.CurrentThread);
        }

        private static List<Type> forbiddenInInsertBlocks = new()
        {
            typeof(BeginBlockComponent)
        };
        private void FillInputBlocks()
        {
            List<BlockBase> res = new List<BlockBase>();
            var blocks = ParametersAllBlocksManager.FactoryStyles;
            BlockSchemeFromViewItem temp = new();
            foreach (var item in blocks)
            {
                if (!forbiddenInInsertBlocks.Contains(item.BlockComponent.GetType()))
                {
                    BlockBase blockBase = temp.CreateBlock(item);
                    blockBase.ToolTip = new ToolTip() { Content = GetHotButtons(item.BlockComponent) };
                    ToolTipService.SetShowOnDisabled((ToolTip)blockBase.ToolTip, true);
                    blockBase.IsEnabled = false;
                    res.Add(blockBase);
                }
            }
            InputBlocks = res;
        }

        private void CreateStandartScheme()
        {
            Schemes.Add(new BlockSchemeFromViewItem() { RootComponent = (BeginBlockComponent)BlockComponentOperations
                .Deserialize(Encoding.UTF8.GetString(Resources.Resources.StartScheme)) });
        }

        private bool ReadInstructionsLanguange()
        {
            PatternLaguangeList.ReadInstruactions();
            if (PatternLaguangeList.Instructions.Count == 0)
                return false;
            try
            {
                converter.Read(PatternLaguangeList.Instructions[0].FullName);
                SelectInstruction = PatternLaguangeList.Instructions[0];
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion //Constructions

        #region Converter
        //конвертер для конвертации 
        private ConverterToBlock converter = new();

        private BlockMemoryLogger MemoryLogger { get; set; }

        private BlockLogger Logger { get; set; }
        #endregion //Converter

        #region Converters
        private BitmapEncoder ConvertToBitmapEncoder(string name) => name.ToLower() switch
        {
            "png" => new PngBitmapEncoder(),
            _ => null
        };

        private static Dictionary<Type, string> hotButtons = new()
        {
            { typeof(BlockComponent), Resources.Resources.ToolTipBlock },
            { typeof(BlockEndComponent), Resources.Resources.ToolTipEndBlock },
            { typeof(BlockMethodComponent), Resources.Resources.ToolTipMethodBlock},
            { typeof(BlockInputComponent), Resources.Resources.ToolTipInputBlock },
            { typeof(BlockOutputComponent), Resources.Resources.ToolTipOutputBlock },
            { typeof(BlockCycleComponent), Resources.Resources.ToolTipCycleBlock},
            { typeof(BlockPostCycleComponent), Resources.Resources.ToolTipPostCycleBlock},
            { typeof(BlockForComponent), Resources.Resources.ToolTipForCycleBlock},
            { typeof(BlockIfComponent), Resources.Resources.ToolTipIfBlock},
            { typeof(BlockSwitchComponent), Resources.Resources.ToolTipSwitchBlock }


        };
        private static string GetHotButtons(IBlockComponent component) 
        {
            string res;
            if (hotButtons.TryGetValue(component.GetType(), out res))
                return res;
            throw new ArgumentException();
        }

        #endregion //Converters

        #region Tools

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string str = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(str));
        private void SetDialog(string message)
        {
            Dialog += message + '\n';
        }
        private void SetDialogError(string message)
        {
            Dialog += message + '\n';
        }
        #endregion//Tools
    }
}
