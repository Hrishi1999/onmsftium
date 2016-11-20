using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Methane
{
    class ViewModel
    {
        public ObservableCollection<HarmonicKernel.Core.ArticleItem> result; //ObservableCollection of Articles 
        public ObservableCollection<HarmonicKernel.Core.ArticleItem> Result
        {
            get
            {
                return result;
            }
            set
            {
                result = value;
            }
        }
        public ObservableCollection<HarmonicKernel.Core.PodcastItem> p_result; //ObservableCollection of Podcasts 
        public ObservableCollection<HarmonicKernel.Core.PodcastItem> P_result
        {
            get
            {
                return p_result;
            }
            set
            {
                p_result = value;
            }
        }
        public ObservableCollection<HarmonicKernel.Core.VideoItem> v_result; //ObservableCollection of Videos 
        public ObservableCollection<HarmonicKernel.Core.VideoItem> V_result
        {
            get
            {
                return v_result;
            }
            set
            {
                v_result = value;
            }
        }
    }

}
