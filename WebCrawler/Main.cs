
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Ivony.Html.Parser;
using Ivony.Html;
using Ivony.Fluent;
using System.IO;
using System.Dynamic;
using System.Collections.Specialized;
using System.Threading;
using System.Xml.Linq;
using LitJson;
using System.Collections;

namespace WebCrawler
{
    public partial class Main : Form
    {
        string _configPath = "";
        JsonData _config;
        dynamic _db = new ExpandoObject();
        public Main()
        {
            InitializeComponent();

            _configPath = System.Windows.Forms.Application.StartupPath + "\\" + System.Configuration.ConfigurationManager.AppSettings["configPath"] + "\\";

            DirectoryInfo _folder = new DirectoryInfo(_configPath);
            var _files = _folder.GetFiles();
            foreach (var cfg in _files)
            {
                this.cbSiteConfig.Items.Add(cfg.Name);
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (_config == null)
            {
                MessageBox.Show("请选择站点配置");
                return;
            }

            int _startPage = int.Parse(this.tbstartPage.Text);
            int _endPage = int.Parse(this.tbendpage.Text);
            int _sleep = int.Parse(_config["sleep"].ToString());

            this.button1.Enabled = false;

            Thread _thread = new Thread(new ParameterizedThreadStart(o =>
            {
                for (int page = _startPage; page < _endPage; page++)
                {
                    try
                    {
                        var _content = this.GetData(page);

                        using (Txooo.TxDataHelper helper = Txooo.TxDataHelper.GetDataHelper(_db.DbNode))
                        {
                            foreach (Hashtable con in _content)
                            {
                                try
                                {
                                    var _childHashList = (List<Hashtable>)con["child_relation"];
                                    con.Remove("child_relation");
                                    var _id = 0;
                                    if (!string.IsNullOrEmpty(_db.Main))
                                    {
                                        _id = Convert.ToInt32(helper.SqlScalar(_db.Main, con));
                                    }

                                    if (!string.IsNullOrEmpty(_db.Child)
                                        && bool.Parse(_config["child"]["enable"].ToString()))
                                    {
                                        foreach (var child in _childHashList)
                                        {
                                            if (_db.NeedMainId)
                                            {
                                                if (_id > 0)
                                                {
                                                    child["@ID"] = _id;
                                                    int _r = helper.SqlExecute(_db.Child, child);
                                                }
                                            }
                                            else
                                            {
                                                int _r = helper.SqlExecute(_db.Child, child);
                                            }
                                        }
                                    }

                                }
                                catch (Exception ex)
                                {
                                    continue;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        continue;
                    }
                    Thread.Sleep(_sleep);
                }

                this.Invoke(new MethodInvoker(() =>
                {
                    MessageBox.Show("完成");
                    this.button1.Enabled = true;
                }));
            }));

            _thread.IsBackground = true;
            _thread.Start(null);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (_config == null)
            {
                MessageBox.Show("请选择站点配置");
                return;
            }
            this.tbCheck.Clear();
            this.button2.Enabled = false;

            Thread _thread = new Thread(new ParameterizedThreadStart(o =>
            {
                var _content = this.GetData(int.Parse(this.tbstartPage.Text));

                StringBuilder _sb = new StringBuilder();
                foreach (var rowData in _content)
                {
                    foreach (DictionaryEntry kv in rowData)
                    {
                        if ((kv.Value).GetType() == typeof(List<Hashtable>))
                        {
                            foreach (var childRow in (List<Hashtable>)kv.Value)
                            {
                                _sb.Append("*************child*************");
                                _sb.AppendLine();

                                foreach (DictionaryEntry item in childRow)
                                {
                                    _sb.Append(item.Key + "：" + item.Value);
                                    _sb.AppendLine();
                                }
                                _sb.AppendLine();
                            }
                        }
                        else
                        {
                            _sb.Append(kv.Key + "：" + kv.Value);
                            _sb.AppendLine();
                        }
                    }
                    _sb.Append("--------------------------------------------------------------------------------------------------------------------------");
                    _sb.AppendLine();

                    this.Invoke(new MethodInvoker(() => { this.tbCheck.Text=_sb.ToString(); }));
                }
                this.Invoke(new MethodInvoker(() => { this.button2.Enabled = true; }));
            }));

            _thread.IsBackground = true;
            _thread.Start(null);
        }

        private void cbSiteConfig_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                using (StreamReader _sr = new StreamReader(_configPath + this.cbSiteConfig.Text))
                {
                    string _str = _sr.ReadToEnd();
                    _str = _str.Substring(_str.IndexOf("\r\n"));
                    _config = JsonMapper.ToObject(_str);
                }
                string _dbPath = System.Windows.Forms.Application.StartupPath + @"\Data\"
                    + System.Configuration.ConfigurationManager.AppSettings["configPath"] + ".config";

                XDocument _dbDoc = XDocument.Load(_dbPath);
                var _root = _dbDoc.Root;
                _db.DbNode = _root.Element("dbNode").Value;
                _db.Main = _root.Element("main").Value;
                _db.Child = _root.Element("child").Value;
                _db.NeedMainId = bool.Parse(_root.Element("child").Attribute("needMainId").Value);

            }
            catch (Exception ex)
            {
                _config = null;
                MessageBox.Show("加载配置文件出错！");
            }
        }

        private List<Hashtable> GetData(int page)
        { 
            var _url = string.Format(_config["url"].ToString(), page);
            var _encoding = Encoding.GetEncoding(_config["encoding"].ToString());

            var _hashParams = new Hashtable();
            foreach (JsonData field in _config["main"]["rules"])
            {
                _hashParams[field["name"].ToString()] = "";
            }
            var _childParams = new Hashtable();
            foreach (JsonData field in _config["child"]["rules"])
            {
                _childParams[field["name"].ToString()] = "";
            }

            var _mainContentList = new List<Hashtable>();
            var _document = new JumonyParser().LoadDocument(_url, _encoding);
            var _mainMatch = _document.Find(_config["main"]["container"].ToString());
            _mainMatch = _mainMatch.Skip(int.Parse(_config["main"]["skip"].ToString()));

            foreach (var item in _mainMatch)
            {
                try
                {
                    var _content = (Hashtable)_hashParams.Clone();
                    _content["child_relation"] = new List<Hashtable>();

                    var _childUrl = "";
                    foreach (JsonData field in _config["main"]["rules"])
                    {
                        string _rule = field["selector"].ToString();
                        var _value = item.Find(_rule).Skip(int.Parse(field["skip"].ToString())).FirstOrDefault();

                        var _codeType = field.GetDataOrDefault("type") == null ? "text" : "html";

                        var _Str = _codeType == "text" ? _value.InnerText().Trim() : System.Web.HttpUtility.HtmlDecode(_value.InnerHtml().Trim());

                        _content[field["name"].ToString()] = field["filter"].ToString() == "" ?
                            _Str : _Str.Replace(field["filter"].ToString(), "").Trim();

                        if (!string.IsNullOrEmpty(field["childLink"].ToString()))
                        {
                            _childUrl = _value.Attribute(field["childLink"].ToString()).AttributeValue;

                            if (bool.Parse(_config["child"]["enable"].ToString()) &&
                       !string.IsNullOrEmpty(_childUrl))
                            {
                                _childUrl = Helper.Geturl(_childUrl, _config.GetDataOrDefault("baseUrl").ToString());

                                var _childDocument = new JumonyParser().LoadDocument(_childUrl, _encoding);
                                var _itemCount = _childDocument.Find(_config["child"]["container"].ToString());
                                _itemCount = _itemCount.Skip(int.Parse(_config["child"]["skip"].ToString()));

                                foreach (var childItem in _itemCount)
                                {
                                    var _childContent = (Hashtable)_childParams.Clone();
                                    foreach (JsonData child in _config["child"]["rules"])
                                    {
                                        string _crule = child["selector"].ToString();
                                        var _ruleAry = _crule.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                        string _cvalue = string.Empty;
                                        foreach (string r in _ruleAry)
                                        {
                                            var _el = childItem.Find(r).FirstOrDefault();
                                            if (_el != null)
                                            {
                                                _cvalue = _el.InnerText().Trim();
                                                break;
                                            }
                                        }

                                        _childContent[child["name"].ToString()] = (child["filter"].ToString() == "" ?
                                _cvalue : _cvalue.Replace(child["filter"].ToString(), "")).Trim('\r', '\n');
                                    }

                                    ((List<Hashtable>)_content["child_relation"]).Add(_childContent);

                                    if (!bool.Parse(_config["child"]["multi"].ToString()))
                                        break;
                                }
                            }
                        }
                    }
                    _mainContentList.Add(_content);
                }
                catch (Exception ex)
                {
                    continue;
                }
            }
            return _mainContentList;
        }

        private void tbstartPage_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != 8 && !Char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void tbendpage_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != 8 && !Char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private T Cfg<T>(string node1, params string[] nodes)
        {
            try
            {
                var _cfg = _config[node1];

                foreach (string item in nodes)
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        _cfg = _cfg[item];
                    }
                }

                return (T)Convert.ChangeType(_cfg.ToString(), typeof(T));
            }
            catch (Exception ex)
            {
                return default(T);
            }
        }
    }
}
