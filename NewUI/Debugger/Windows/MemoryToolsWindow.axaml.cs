#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Mesen.Debugger.Controls;
using Mesen.Debugger.ViewModels;
using Mesen.Interop;
using System.ComponentModel;
using Avalonia.Interactivity;
using System.Collections.Generic;
using Mesen.Debugger.Utilities;
using System.IO;
using Mesen.Utilities;
using Mesen.Localization;

namespace Mesen.Debugger.Windows
{
	public class MemoryToolsWindow : Window
	{
		private NotificationListener _listener;
		private HexEditor _editor;
		private MemoryToolsViewModel _model;

		public MemoryToolsWindow()
		{
			InitializeComponent();
#if DEBUG
			this.AttachDevTools();
#endif

			if(Design.IsDesignMode) {
				return;
			}

			_editor = this.FindControl<HexEditor>("Hex");
			_editor.ByteUpdated += editor_ByteUpdated;

			_listener = new NotificationListener();
			_listener.OnNotification += listener_OnNotification;
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			_listener?.Dispose();
			_model.Config.SaveWindowSettings(this);
			_model.SaveConfig();
			base.OnClosing(e);
		}

		private void InitializeComponent()
		{
			AvaloniaXamlLoader.Load(this);
		}

		private void OnSettingsClick(object sender, RoutedEventArgs e)
		{
			_model.Config.ShowOptionPanel = !_model.Config.ShowOptionPanel;
		}

		private async void OnLoadTblFileClick(object sender, RoutedEventArgs e)
		{
			string? filename = await FileDialogHelper.OpenFile(null, VisualRoot, FileDialogHelper.TblExt);
			if(filename != null) {
				_model.TblConverter = TblLoader.Load(File.ReadAllLines(filename));
			}
		}

		private void editor_ByteUpdated(object? sender, ByteUpdatedEventArgs e)
		{
			DebugApi.SetMemoryValue(_model.Config.MemoryType, (uint)e.ByteOffset, e.Value);
		}

		protected override void OnDataContextChanged(EventArgs e)
		{
			if(this.DataContext is MemoryToolsViewModel model) {
				_model = model;
				_model.Config.LoadWindowSettings(this);

				_model.SetActions(new object[] {
					new ContextMenuAction() {
						Name = ResourceHelper.GetMessage("Copy"),
						IconFile = "Assets/Copy.png",
						IsEnabled = () => _editor.SelectionLength > 0,
						OnClick = () => _editor.CopySelection()
					},
					new ContextMenuAction() {
						Name = ResourceHelper.GetMessage("Paste"),
						IconFile = "Assets/Paste.png",
						OnClick = () => _editor.PasteSelection()
					},
					new Separator(),
					new ContextMenuAction() {
						Name = ResourceHelper.GetMessage("SelectAll"),
						IconFile = "Assets/SelectAll.png",
						OnClick = () => _editor.SelectAll()
					},
				});
			} else {
				throw new Exception("Invalid model");
			}
		}

		private void listener_OnNotification(NotificationEventArgs e)
		{
			if(e.NotificationType == ConsoleNotificationType.PpuFrameDone || e.NotificationType == ConsoleNotificationType.CodeBreak) {
				Dispatcher.UIThread.Post(() => {
					_editor.InvalidateVisual();
				});
			}
		}
	}
}
