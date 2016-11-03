/*----------------------------------------------------------------
 *  Copyright (C) 2015 天下商机（txooo.com）版权所有
 * 
 *  文 件 名：Helper
 *  所属项目：
 *  创建用户：张德良
 *  创建时间：2016/7/12 星期二 下午 16:49:14
 *  
 *  功能描述：
 *          1、
 *          2、 
 * 
 *  修改标识：  
 *  修改描述：
 *  待 完 善：
 *          1、 
----------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCrawler
{
    public class Helper
    {
        public static string Geturl(string urlX, string baseUrl)
        {
            if (urlX.IndexOf("//") == -1)
            {
                Uri baseUri = new Uri(baseUrl);
                Uri absoluteUri = new Uri(baseUri, urlX);
                return absoluteUri.ToString();
            }
            return urlX;
        }
    }
}
