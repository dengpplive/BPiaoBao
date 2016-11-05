using System;
using System.Collections.Generic;
using System.Text;
using PnrAnalysis.Model;
namespace PnrAnalysis
{
    /// <summary>
    /// 返回指令实体
    /// </summary>
    ///  
    [Serializable]
    public class InsModel
    {
        public FDModel _fd = new FDModel();
        public AVHModel _avh = new AVHModel();
        public PatModel _pat = new PatModel();
        public DetrModel _detr = new DetrModel();

    }
}
