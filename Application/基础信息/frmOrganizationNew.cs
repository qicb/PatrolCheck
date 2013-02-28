﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace WorkStation
{
    public partial class frmOrganizationNew : Form
    {
        public frmOrganizationNew()
        {
            InitializeComponent();
        }
        private bool isEdit=false;
        private object orgID = null;
        private object organzation_id = null;
        /// <summary>
        /// 父组织ID-新建
        /// </summary>
        public object Organzation_ID
        {
            get { return organzation_id; }
            set { organzation_id = value; }
        }
        /// <summary>
        /// 是否是编辑
        /// </summary>
        public bool IsEdit
        {
            get { return isEdit; }
            set { isEdit = value; }
        }
        /// <summary>
        /// 组织ID-编辑时传递
        /// </summary>
        public object OrgID
        {
            get { return orgID; }
            set { orgID = value; }
        }
        private void frmOrganizationNew_Load(object sender, EventArgs e)
        {
            BindComboBox();
        }

        private void BindComboBox()
        {
            using (SqlDataReader dr = SqlHelper.ExecuteReader("Select Code,Meaning from Codes Where Purpose='ValidState'"))
            {
                while (dr.Read())
                {
                    cboValidState.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem(dr["Meaning"].ToString(), dr["Code"], -1));
                }
                cboValidState.EditValue = 1;
            }

            using (SqlDataReader dr = SqlHelper.ExecuteReader("Select Code,Meaning from Codes Where Purpose='OrgType'"))
            {
                while (dr.Read())
                {
                    cboOrgType.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem(dr["Meaning"].ToString(), dr["Code"], -1));
                }
                cboOrgType.EditValue = 1;
            }
        }

        private void btnChoseArea_Click(object sender, EventArgs e)
        {
            frmOrganizationNewAreaChose frmChose = new frmOrganizationNewAreaChose();
            frmChose.ShowDialog();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            if (tbName.Text == "")
            {
                MessageBox.Show("请确定无空值"); return;
            }
            string sql = "Insert Into Organization(Name,Organization_id,OrgType,ValidState) Values(@name,@organization_id,@orgtype,@validstate)";
            if (IsEdit)
            {
                sql = "Update Organization Set Name=@name,OrgType=@orgtype,ValidState=@validstate where ID=@id";
            }
            SqlParameter[] pars = new SqlParameter[]{
               new SqlParameter("@name",tbName.Text),
               new SqlParameter("@orgtype",cboOrgType.EditValue),
               new SqlParameter("@organization_id",organzation_id),
               new SqlParameter("@validstate",cboValidState.EditValue),
               new SqlParameter("@id",OrgID)
            };
            if (SqlHelper.ExecuteNonQuery(sql, pars) > 0)
            {
                MessageBox.Show("保存成功");
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
