using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VIN_DIZEL
{
    public partial class Main_menu : Form
    {

        List<Dictionary<string, string>> slovar = new List<Dictionary<string, string>>();
        Dictionary<string, string> dict;
        string my_key;
        public string big_image;
        public void Seek()
        {

            foreach (var slovo in slovar)
            {

                if ((slovo["OrderNumber"] == my_key) || (slovo["VIN"] == my_key) || (slovo["Id"] == my_key))
                {

                    dict = slovo;
                    string my_request = slovo["Url"].Replace("http:", "https:");
                    string model = slovo["ModelSysName"].Replace("new-", "");
                    string order = slovo["OrderNumber"];

                    if (slovo["Url"][slovo["Url"].Length - 1] != '/')
                        my_request += "/";
                    if (slovo["Url"].Contains("CarDetails") && slovo["Model"].Contains("Maybach"))

                        my_request = $@"https://sales.mercedes-cardinal.ru/car/{model.Split('-')[1] + "-suv-" + model.Split('-')[0] }/{order}/";
                    else if (slovo["Url"].Contains("CarDetails") && slovo["Model"].Contains("Новый"))
                        my_request = $@"https://sales.mercedes-cardinal.ru/amg/{model}/{order}/";
                    else if (my_request.Contains("CarDetails"))
                    {
                        my_request = $@"https://sales.mercedes-cardinal.ru/car/{model}/{order}/";
                    }

                    HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(my_request);
                    request.Method = "HEAD";
                    request.AllowAutoRedirect = false;

                        try
                        {


                    using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                    {
                        if (response.StatusCode != HttpStatusCode.OK)
                            my_request = $@"https://sales.mercedes-cardinal.ru/amg/{model}/{order}/";

                    }

                       }
                      catch 
                      {
                           my_request = $@"https://sales.mercedes-cardinal.ru/car/{model.Split('-')[1] +  model.Split('-')[0] }/{order}/";
                       }

                    MyBox.Text = my_request;
                    listBox1.DataSource = dict.Keys.ToList();
                    dict.Count();
                    //При изменении индекса выбранного элемента списка показываем соответствующий элемент словаря
                    if (listBox1.Created)
                        listBox2.DataSource = dict.Values.ToList();
                    DataTable dt = new DataTable();

                    dt.Columns.Add("Key", typeof(string));
                    dt.Columns.Add("Value", typeof(string));


                    foreach (KeyValuePair<string, string> kvp in slovo)
                    {
                        DataRow temp = dt.NewRow();

                        temp["Key"] = kvp.Key;
                        temp["Value"] = kvp.Value;
                        dt.Rows.Add(temp);
                    }
                    pictureBox1.ImageLocation = slovo["ListImage"];
                    big_image = slovo["ListImageExteriorBig"];
                }

            }
        }
        public static class Resolver
        {
            private static volatile bool _loaded;

            public static void RegisterDependencyResolver()
            {
                if (!_loaded)
                {
                    AppDomain.CurrentDomain.AssemblyResolve += OnResolve;
                    _loaded = true;
                }
            }

            private static Assembly OnResolve(object sender, ResolveEventArgs args)
            {
                Assembly execAssembly = Assembly.GetExecutingAssembly();
                string resourceName = string.Format("{0}.{1}.dll",
                     execAssembly.GetName().Name,
                    new AssemblyName(args.Name).Name);

                using (System.IO.Stream stream = execAssembly.GetManifestResourceStream(resourceName))
                {
                    if (stream != null)
                    {
                        int read = 0, toRead = (int)stream.Length;
                        byte[] data = new byte[toRead];
                        do
                        {
                            int n = stream.Read(data, read, data.Length - read);
                            toRead -= n;
                            read += n;
                        } while (toRead > 0);
                        return Assembly.Load(data);
                    }
                    return null;

                }
            }
        }

        string url;

        public Main_menu()
        {
            InitializeComponent();
            Resolver.RegisterDependencyResolver();


        }
        public string get_big_image()
        {

            return big_image;
        }

        public async Task My_House()
        {



            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

            var client = new HttpClient();



            HttpResponseMessage httpResponseMessage = await client.GetAsync(url);

            string responseBody = await httpResponseMessage.Content.ReadAsStringAsync();
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                JToken jArray = new JArray();
                List<string> list = jArray.ToObject<List<string>>();


                JToken jObject = JToken.Parse(responseBody);
                slovar.Clear();
        
                dataGridView1.Rows.Clear();
                foreach (var item in jObject)
          
                {
                    slovar.Add(JObject.FromObject(item).ToObject<Dictionary<string, string>>());
                }//
                /* listBox3.Items.Clear();
                 listBox4.Items.Clear();
                 listBox5.Items.Clear();*/
                System.Threading.Thread TMyfunc = new System.Threading.Thread(delegate ()
                {
                    Data_Insert();
                });
                TMyfunc.Start();
                /*
                listBox3.SelectedIndexChanged += (s, e) => 
                { textBox2.Text = listBox3.SelectedItem.ToString();
                    listBox4.SelectedIndex = listBox3.SelectedIndex;
                    listBox5.SelectedIndex = listBox3.SelectedIndex;
                    button1.PerformClick();
                };
                label3.Text = listBox3.Items.Count.ToString();
            }
            
            */
   
                button1.PerformClick();
            }
          

        }
            private void Data_Insert()
            {
                /*  foreach (var slovo in slovar)
                  {

                      listBox5.Items.Add(slovo["FinalPrice"]);
                      listBox3.Items.Add(slovo["OrderNumber"]);
                      listBox4.Items.Add(listBox3.Items.Count);
                  }*/
                DataTable table = new DataTable();


            dataGridView1.RowHeadersVisible = false;
   
                table.Columns.Add("Заказ");
                table.Columns.Add("Стоимость");
            table.Columns.Add("Наименование");

            int my_int = 1;

                foreach (var slovo in slovar)
                {
                    DataRow row = table.NewRow();

                    row[0] = slovo["OrderNumber"];
                    row[1] = slovo["FinalPrice"];
                row[2] = slovo["Engine"];
                    table.Rows.Add(row);
                    my_int++;
                }
                dataGridView1.DataSource = table;
            label3.Text = (my_int -1).ToString();

            dataGridView1.Columns[0].Width = 75;
            dataGridView1.Columns[1].Width = 60;
            dataGridView1.Columns[2].Width = 180;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.Sort(dataGridView1.Columns[1], System.ComponentModel.ListSortDirection.Ascending);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            my_key = textBox2.Text;
            Seek();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox2.SetSelected(listBox1.SelectedIndex, true);
        }

        private void listBox2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Clipboard.SetText(listBox2.SelectedItem.ToString());
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                url = "https://cars.mercedes-benz.ru/api/Cars/GetList?cityId=49";
                Task.Run(My_House);
            }
            else if (radioButton2.Checked)
            {
                url = "https://cars.mercedes-benz.ru/api/Cars/GetList?";
                Task.Run(My_House);
            }

        }

        private void Main_menu_Load(object sender, EventArgs e)
        {
            radioButton1.PerformClick();
            button2.PerformClick();


        }

        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
        
                textBox2.Text = dataGridView1.Rows?[e.RowIndex]?.Cells[0]?.Value.ToString();
                button1.PerformClick();
        
        
        }
    }
}
