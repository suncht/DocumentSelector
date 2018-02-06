using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DocumentSelector
{
    class TipListBox : ListBox  
    {
        ToolTip tip;

        public TipListBox()
        {
            tip = new ToolTip();
        }

        private void SetTipMessage(string strTip)
        {
            tip.SetToolTip(this, strTip);
        }
        /// <summary>  
        /// 重写鼠标移动事件  
        /// </summary>  
        /// <param name="e"></param>  
        //protected override void OnMouseMove(MouseEventArgs e)
        //{
        //    base.OnMouseMove(e);

        //    int idx = IndexFromPoint(e.Location);// 获取鼠标所在的项索引  
        //    if (idx == -1) //鼠标所在位置没有 项  
        //    {
        //        SetTipMessage(""); //设置提示信息为空  
        //        return;
        //    }
        //   //string txt = GetItemText(this.Items[idx]); //获取项文本 
        //   Item item = this.Items[idx] as Item;
        //   if (item!=null)
        //       SetTipMessage("文件名：" + item.value); //设置提示信息  
        //}
    }
}
