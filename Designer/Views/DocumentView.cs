﻿using System;
using System.Collections.Generic;
using FreeSCADA.Common;
using FreeSCADA.Interfaces;
using FreeSCADA.Common.Gestures;
using FreeSCADA.Designer.SchemaEditor.Tools;
using WeifenLuo.WinFormsUI.Docking;
using FreeSCADA.CommonUI.Interfaces;

namespace FreeSCADA.Designer.Views
{
	abstract class DocumentView : DockContent, IDocumentView
	{

        ISelectionManager _selManager;
        public virtual ISelectionManager SelectionManager
        {
            get
            {
                if (_selManager==null)
                   _selManager=new DummySelectionManager();
                return _selManager;
            }
            protected set { _selManager = value; }
        }
        
        public virtual ITool ActiveTool
        {
            get;
            set;
        }


        public virtual  MapZoom ZoomManager
        {
            get;
            protected set;
        }
        public virtual System.Windows.Controls.Panel MainPanel
        {
            get;
            protected set;
        }
        
        string documentName = "";
		bool modifiedFlag;
		bool handleModifiedFlagOnClose = true;

        public event EventHandler IsModifiedChanged;

      
		public virtual List<CommandInfo> DocumentCommands
        {
            get;
            protected set;
        }

        public IUndoBuffer UndoBuff
        {
            get;
            protected set;
        }

		public DocumentView(IDocument doc)
		{
			DocumentCommands = new List<CommandInfo>();
			DockAreas = DockAreas.Float | DockAreas.Document;
            this.Document = doc ;
			UpdateCaption();
		}
        public IDocument Document
        {
            get;
            protected set;
        }
        public string DocumentName
        {
            get { return Document.Name; }
            set
            {
                Document.Save(value);
                UpdateCaption();
            }
        }

		/// <summary>
		/// This property should be set to "true" for new documents and set to "false" after saving the document.
		/// </summary>
		public virtual bool IsModified
		{
			get { return modifiedFlag; }
			set { modifiedFlag = value;
                if (IsModifiedChanged != null)
                    IsModifiedChanged(this, new EventArgs());
                UpdateCaption(); }
		}

		public bool HandleModifiedOnClose
		{
			get { return handleModifiedFlagOnClose; }
			set { handleModifiedFlagOnClose = value; }
		}

        public virtual void OnToolActivated(object sender, Type tool)
        {
        }

		public virtual void OnActivated()
		{
			foreach (CommandInfo cmdInfo in DocumentCommands)
                foreach(string c in cmdInfo.defaultContextes)
                    Env.Current.Commands.GetContext(c).AddCommand(cmdInfo.command);
		}

		public virtual void OnDeactivated()
		{
            foreach (CommandInfo cmdInfo in DocumentCommands)
                foreach (string c in cmdInfo.defaultContextes)
                    Env.Current.Commands.GetContext(c).RemoveCommand(cmdInfo.command);
			
        }

		public virtual bool SaveDocument()
		{
			return false;
		}
	

        

		protected virtual void UpdateCaption()
		{
			TabText = DocumentName;
			if (IsModified)
				TabText += " *";
		}

        
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            SelectionManager.SelectObject(null);    
        }

        public virtual void OnPropertiesBrowserChanged(object el)
        {
            IsModified = true;
        }
    }
}
