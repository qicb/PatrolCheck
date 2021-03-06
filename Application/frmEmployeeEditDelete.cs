using System;
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
    public partial class frmEmployeeEditDelete : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        public frmEmployeeEditDelete()
        {
            InitializeComponent();
        }      
        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {            
            string selectEmpoyee = "select Employee.Name emName,Employee.Alias alias,Rfid.Name Name,Rfid.ID,Post.Name postName,(select meaning from codes where code=Employee.validstate and purpose='validstate') as ValidState from Employee left join Rfid on Employee.Rfid_ID=Rfid.ID left join Post_Employee on  Employee.ID=Post_Employee.Employee_ID left join Post on  Post_Employee.Post_ID=Post.ID where Employee.ID=@id";
            SqlParameter[] par = new SqlParameter[] { new SqlParameter("@id", this.dgvEmployessDel.GetRowCellValue      (dgvEmployessDel.FocusedRowHandle,"ID")) };
            SqlDataReader dr = SqlHelper.ExecuteReader(selectEmpoyee,par);
            while(dr.Read())
            {
                this.txtName.Text = dr[0].ToString();
                this.txtAlias.Text = dr[1].ToString();               
                this.txtRelation.Text = dr[2].ToString();  
                this.txtRelation.Tag=dr[3].ToString();
                this.cboPost.Text = dr[4].ToString();
                this.cboState.Text = dr[5].ToString();
            }
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            string del = "";
            string delCard = "delete from  Employee  where  ID in (";
            for (int i = 0; i < dgvEmployessDel.DataRowCount; i++)
            {
                object idCheck = dgvEmployessDel.GetRowCellValue(i, gridColumn_check);
                if (idCheck != null && (bool)idCheck == true)
                {
                    del += dgvEmployessDel.GetRowCellValue(i, "ID").ToString() + ",";
                }
            }
            if (del != "")
            {
                del = del.Substring(0, del.Length - 1);
                delCard += del + ")";
                int i = SqlHelper.ExecuteNonQuery(delCard);
                if (i > 0)
                {
                    MessageBox.Show("删除成功！");
                }
                else
                {
                    MessageBox.Show("删除失败！");
                }
            }
            else
            {
                MessageBox.Show("请选择要删除的项");
            }
            BindEmployee();     
           
        }
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (this.txtName.Text == "")
            {
                MessageBox.Show("人员名称不能为空", "友情提示", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                this.txtName.Focus();
            }
            else if (txtAlias.Text == "")
            {
                MessageBox.Show("人员别名不能为空", "友情提示", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                this.txtAlias.Focus();
            }
             else if (this.cboPost.SelectedValue.ToString() == null)
             {
                 MessageBox.Show("所属岗位不能为空", "友情提示", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                 this.cboPost.Focus();
             }

             else
             {
                 string UpdateEmployee = "update Employee set Employee.Name=@name,Employee.Alias=@alias,Rfid_ID=@rfid_id,Employee.ValidState=@ValidState where Employee.ID=@id;select  @@identity";
                 SqlParameter[] par = new SqlParameter[]{ new SqlParameter("@id",this.dgvEmployessDel.GetRowCellValue      (dgvEmployessDel.FocusedRowHandle, "ID")),
                                                          new SqlParameter("@name",SqlDbType.NVarChar),
                                                          new SqlParameter("@alias",SqlDbType.NVarChar),
                                                          new SqlParameter("@rfid_id",SqlDbType.BigInt),
                                                          new SqlParameter("@ValidState",SqlDbType.Int)};              
                 par[1].Value = this.txtName.Text;
                 par[2].Value = this.txtAlias.Text;
                 if ((int)SqlHelper.ExecuteScalar("Select Count(1) From Rfid Where Purpose=1 and validstate=1 and ID='" + this.txtRelation.Tag + "'") == 1)
                 {
                    par[3].Value =int.Parse(this.txtRelation.Tag.ToString());                  
                 }
                 else
                 {
                     MessageBox.Show("请确保存在此标签卡");
                     return;
                 }               
                 par[4].Value = this.cboState.SelectedValue;
                 string a =SqlHelper.ExecuteScalar(UpdateEmployee, par).ToString();
                 if (a !=null)
                 {
                     MessageBox.Show("更新成功！");
                 }
                 else
                 {
                     MessageBox.Show("更新失败！");
                 }
                 string UpdateEmPost = "update Post_Employee set Post_ID=@id where Employee_ID=@emID";
                 SqlParameter[] par2 = new SqlParameter[]
                 {
                   new SqlParameter("@emID",SqlDbType.Int),
                   new SqlParameter("@id",SqlDbType.Int)
                  };         
                 par2[0].Value = this.dgvEmployessDel.GetRowCellValue(dgvEmployessDel.FocusedRowHandle, "ID");
                 par2[1].Value = cboPost.SelectedValue.ToString();
                 int i = SqlHelper.ExecuteNonQuery(UpdateEmPost, par2);
             }
            BindEmployee();
        }
        /// <summary>
        /// 取消
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        /// <summary>
        /// 数据加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmEditOrDeleteEmployee_Load(object sender, EventArgs e)
        {
            string selectPost = "select * from Post where ValidState=1";
            DataSet ds = SqlHelper.ExecuteDataset(selectPost);
            cboPost.DataSource = ds.Tables[0];
            cboPost.DisplayMember = "Name";
            cboPost.ValueMember = "ID";        

            string selectState = "select Code,Meaning from Codes where Purpose='ValidState' ";
            DataSet dse = SqlHelper.ExecuteDataset(selectState);
            cboState.DataSource = dse.Tables[0];
            cboState.DisplayMember = "Meaning";
            cboState.ValueMember = "Code";
            BindEmployee();
        }
        /// <summary>
        /// 数据绑定
        /// </summary>
        public void BindEmployee()
        {          
            string selectEmployee = "select Employee.ID,Employee.Name emName,Employee.Alias alias,Rfid.Name Name,Post.Name  postName,(select meaning from codes where code=Employee.validstate and purpose='validstate') as ValidState from Employee left join Rfid on  Employee.Rfid_ID=Rfid.ID left join Post_Employee on Employee.ID=Post_Employee.Employee_ID left join Post on Post_Employee.Post_ID=Post.ID ";
            DataSet ds = SqlHelper.ExecuteDataset(selectEmployee);
            ds.Tables[0].Columns.Add(new DataColumn("check", typeof(System.Boolean)));
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                ds.Tables[0].Rows[i]["check"] = false;
            }
            this.gridControl1.DataSource = ds.Tables[0];
        }
        /// <summary>
        /// 重新加载
        /// </summary>
        /// <param name="e"></param>
        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            BindEmployee();
        }

        private void btnChose_Click(object sender, EventArgs e)
        {
            frmPointChoseRfid f = new frmPointChoseRfid();
            f.SelIndex = 1;
            f.ShowDialog();
            this.txtRelation.Text = f.RFID_Name == null ? null : f.RFID_Name.ToString();
            this.txtRelation.Tag = f.RFID_ID;
            this.btnSave.Enabled = true;
            this.txtRelation.ReadOnly = false;       
        }
    }
}
