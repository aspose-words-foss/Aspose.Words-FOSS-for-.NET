// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 23/09/2014 by Konstantin Sidorenko

using System;
using System.Data;
using System.Xml;
using Aspose.JavaAttributes;

namespace Aspose.TestFx.Pal 
{
    /// <summary>
    /// This code is a typed dataset and was generated using a Microsoft tool.
    /// But it is not going to be regenerated because we only need it in a small number of unit tests.
    /// </summary>
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Diagnostics.DebuggerStepThrough()]
    [System.ComponentModel.ToolboxItem(true)]
    [JavaManual()]
    public class DBNullDataSet : DataSet
    {
        
        private CustomerDataTable tableCustomer;
        
        public DBNullDataSet() {
            this.InitClass();
            System.ComponentModel.CollectionChangeEventHandler schemaChangedHandler = new System.ComponentModel.CollectionChangeEventHandler(this.SchemaChanged);
            this.Tables.CollectionChanged += schemaChangedHandler;
            this.Relations.CollectionChanged += schemaChangedHandler;
        }
        
        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibilityAttribute(System.ComponentModel.DesignerSerializationVisibility.Content)]
        public CustomerDataTable Customer {
            get {
                return this.tableCustomer;
            }
        }
        
        public override DataSet Clone() {
            DBNullDataSet cln = ((DBNullDataSet)(base.Clone()));
            cln.InitVars();
            return cln;
        }
        
        protected override bool ShouldSerializeTables() {
            return false;
        }
        
        protected override bool ShouldSerializeRelations() {
            return false;
        }
        
        protected override void ReadXmlSerializable(XmlReader reader) {
            this.Reset();
            DataSet ds = new DataSet();
            ds.ReadXml(reader);
            if ((ds.Tables["Customer"] != null)) {
                this.Tables.Add(new CustomerDataTable(ds.Tables["Customer"]));
            }
            this.DataSetName = ds.DataSetName;
            this.Prefix = ds.Prefix;
            this.Namespace = ds.Namespace;
            this.Locale = ds.Locale;
            this.CaseSensitive = ds.CaseSensitive;
            this.EnforceConstraints = ds.EnforceConstraints;
            this.Merge(ds, false, System.Data.MissingSchemaAction.Add);
            this.InitVars();
        }
        
        protected override System.Xml.Schema.XmlSchema GetSchemaSerializable() {
            System.IO.MemoryStream stream = new System.IO.MemoryStream();
            this.WriteXmlSchema(new XmlTextWriter(stream, null));
            stream.Position = 0;
            return System.Xml.Schema.XmlSchema.Read(new XmlTextReader(stream), null);
        }
        
        internal void InitVars() {
            this.tableCustomer = ((CustomerDataTable)(this.Tables["Customer"]));
            if ((this.tableCustomer != null)) {
                this.tableCustomer.InitVars();
            }
        }
        
        private void InitClass() {
            this.DataSetName = "Dataset1";
            this.Prefix = "";
            this.Namespace = "";
            this.Locale = new System.Globalization.CultureInfo("en-NZ", false);
            this.CaseSensitive = false;
            this.EnforceConstraints = true;
            this.tableCustomer = new CustomerDataTable();
            this.Tables.Add(this.tableCustomer);
        }
        
        private bool ShouldSerializeCustomer() {
            return false;
        }
        
        private void SchemaChanged(object sender, System.ComponentModel.CollectionChangeEventArgs e) {
            if ((e.Action == System.ComponentModel.CollectionChangeAction.Remove)) {
                this.InitVars();
            }
        }
        
        public delegate void CustomerRowChangeEventHandler(object sender, CustomerRowChangeEvent e);
        
        [System.Diagnostics.DebuggerStepThrough()]
        public class CustomerDataTable : DataTable, System.Collections.IEnumerable {
            
            private DataColumn columnF1;
            
            private DataColumn columnF2;
            
            internal CustomerDataTable() : 
                    base("Customer") {
                this.InitClass();
            }
            
            internal CustomerDataTable(DataTable table) : 
                    base(table.TableName) {
                if ((table.CaseSensitive != table.DataSet.CaseSensitive)) {
                    this.CaseSensitive = table.CaseSensitive;
                }
                if ((table.Locale.ToString() != table.DataSet.Locale.ToString())) {
                    this.Locale = table.Locale;
                }
                if ((table.Namespace != table.DataSet.Namespace)) {
                    this.Namespace = table.Namespace;
                }
                this.Prefix = table.Prefix;
                this.MinimumCapacity = table.MinimumCapacity;
                this.DisplayExpression = table.DisplayExpression;
            }
            
            [System.ComponentModel.Browsable(false)]
            public int Count {
                get {
                    return this.Rows.Count;
                }
            }
            
            internal DataColumn F1Column {
                get {
                    return this.columnF1;
                }
            }
            
            internal DataColumn F2Column {
                get {
                    return this.columnF2;
                }
            }
            
            public CustomerRow this[int index] {
                get {
                    return ((CustomerRow)(this.Rows[index]));
                }
            }
            
            public event CustomerRowChangeEventHandler CustomerRowChanged;
            
            public event CustomerRowChangeEventHandler CustomerRowChanging;
            
            public event CustomerRowChangeEventHandler CustomerRowDeleted;
            
            public event CustomerRowChangeEventHandler CustomerRowDeleting;
            
            public void AddCustomerRow(CustomerRow row) {
                this.Rows.Add(row);
            }
            
            public CustomerRow AddCustomerRow(string F1, string F2) {
                CustomerRow rowCustomerRow = ((CustomerRow)(this.NewRow()));
                rowCustomerRow.ItemArray = new object[] {
                        F1,
                        F2};
                this.Rows.Add(rowCustomerRow);
                return rowCustomerRow;
            }
            
            public System.Collections.IEnumerator GetEnumerator() {
                return this.Rows.GetEnumerator();
            }
            
            public override DataTable Clone() {
                CustomerDataTable cln = ((CustomerDataTable)(base.Clone()));
                cln.InitVars();
                return cln;
            }
            
            protected override DataTable CreateInstance() {
                return new CustomerDataTable();
            }
            
            internal void InitVars() {
                this.columnF1 = this.Columns["F1"];
                this.columnF2 = this.Columns["F2"];
            }
            
            private void InitClass() {
                this.columnF1 = new DataColumn("F1", typeof(string), null, System.Data.MappingType.Element);
                this.Columns.Add(this.columnF1);
                this.columnF2 = new DataColumn("F2", typeof(string), null, System.Data.MappingType.Element);
                this.Columns.Add(this.columnF2);
            }
            
            public CustomerRow NewCustomerRow() {
                return ((CustomerRow)(this.NewRow()));
            }
            
            protected override DataRow NewRowFromBuilder(DataRowBuilder builder) {
                return new CustomerRow(builder);
            }
            
            protected override System.Type GetRowType() {
                return typeof(CustomerRow);
            }
            
            protected override void OnRowChanged(DataRowChangeEventArgs e) {
                base.OnRowChanged(e);
                if ((this.CustomerRowChanged != null)) {
                    this.CustomerRowChanged(this, new CustomerRowChangeEvent(((CustomerRow)(e.Row)), e.Action));
                }
            }
            
            protected override void OnRowChanging(DataRowChangeEventArgs e) {
                base.OnRowChanging(e);
                if ((this.CustomerRowChanging != null)) {
                    this.CustomerRowChanging(this, new CustomerRowChangeEvent(((CustomerRow)(e.Row)), e.Action));
                }
            }
            
            protected override void OnRowDeleted(DataRowChangeEventArgs e) {
                base.OnRowDeleted(e);
                if ((this.CustomerRowDeleted != null)) {
                    this.CustomerRowDeleted(this, new CustomerRowChangeEvent(((CustomerRow)(e.Row)), e.Action));
                }
            }
            
            protected override void OnRowDeleting(DataRowChangeEventArgs e) {
                base.OnRowDeleting(e);
                if ((this.CustomerRowDeleting != null)) {
                    this.CustomerRowDeleting(this, new CustomerRowChangeEvent(((CustomerRow)(e.Row)), e.Action));
                }
            }
            
            public void RemoveCustomerRow(CustomerRow row) {
                this.Rows.Remove(row);
            }
        }
        
        [System.Diagnostics.DebuggerStepThrough()]
        public class CustomerRow : DataRow {
            
            private readonly CustomerDataTable tableCustomer;
            
            internal CustomerRow(DataRowBuilder rb) : 
                    base(rb) {
                this.tableCustomer = ((CustomerDataTable)(this.Table));
            }
            
            public string F1 {
                get {
                    try {
                        return ((string)(this[this.tableCustomer.F1Column]));
                    }
                    catch (InvalidCastException e) {
                        throw new StrongTypingException("Cannot get value because it is DBNull.", e);
                    }
                }
                set {
                    this[this.tableCustomer.F1Column] = value;
                }
            }
            
            public string F2 {
                get {
                    try {
                        return ((string)(this[this.tableCustomer.F2Column]));
                    }
                    catch (InvalidCastException e) {
                        throw new StrongTypingException("Cannot get value because it is DBNull.", e);
                    }
                }
                set {
                    this[this.tableCustomer.F2Column] = value;
                }
            }
            
            public bool IsF1Null() {
                return this.IsNull(this.tableCustomer.F1Column);
            }
            
            public void SetF1Null() {
                this[this.tableCustomer.F1Column] = System.Convert.DBNull;
            }
            
            public bool IsF2Null() {
                return this.IsNull(this.tableCustomer.F2Column);
            }
            
            public void SetF2Null() {
                this[this.tableCustomer.F2Column] = System.Convert.DBNull;
            }
        }
        
        [System.Diagnostics.DebuggerStepThrough()]
        public class CustomerRowChangeEvent : EventArgs {
            
            private readonly CustomerRow eventRow;
            
            private readonly DataRowAction eventAction;
            
            public CustomerRowChangeEvent(CustomerRow row, DataRowAction action) {
                this.eventRow = row;
                this.eventAction = action;
            }
            
            public CustomerRow Row {
                get {
                    return this.eventRow;
                }
            }
            
            public DataRowAction Action {
                get {
                    return this.eventAction;
                }
            }
        }
    }
}
