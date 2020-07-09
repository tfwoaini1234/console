using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace 自动处理程序
{
    class Program
    {
        static System.Timers.Timer imageTimer = new System.Timers.Timer();
        static System.Timers.Timer DnsTimer = new System.Timers.Timer();
        static System.Timers.Timer phpTimer = new System.Timers.Timer();
        static string dirPath;//qq机器人图片库文件夹
        static List<string> imagePathList = new List<string>();
        static List<string> imageList = new List<string>();
        static string phpcmd;//php合并文件命令
        static string apiUrl = "";
        static string imageUrl = "";
        static long jishu = 0;
        static void Main(string[] args)
        {

            windowsStart();
            //iamgeTimerStart();
            //dnsTimerStart();
            cmdTimerStart();
            Console.ReadLine();
        }

        /// <summary>
        /// 加载配置文件
        /// </summary>
        private static void windowsStart()
        {
            //ReadDirPath("./dirpath.txt");
            //ReadImagePath("./imagedirpath.txt");
            //ReadDNSAPI("./dnsapi.txt");
            //ReadImageUrl("./imageurl.txt");
            ReadPHPMergeFile("./phpcmd.txt");
        }
        /// <summary>
        /// 图片定时器
        /// </summary>
        private static void iamgeTimerStart()
        {
            imageTimer.Elapsed += new ElapsedEventHandler(imageTimerF);
            // 设置引发时间的时间间隔 此处设置为１秒（１０００毫秒）
            imageTimer.Interval = 1000;
            imageTimer.Enabled = true;
            Console.WriteLine("图片服务启动成功");
        }
        /// <summary>
        /// 动态ip解析定时器
        /// </summary>

        private static void dnsTimerStart()
        {
            DnsTimer.Elapsed += new ElapsedEventHandler(changeDNSTimer);
            // 设置引发时间的时间间隔 此处设置为１秒（１０００毫秒）
            DnsTimer.Interval = 1000;
            DnsTimer.Enabled = true;
            Console.WriteLine("动态域名解析服务启动成功");
        }
        /// <summary>
        /// php命令行定时器
        /// </summary>
        private static void cmdTimerStart()
        {
            phpTimer.Elapsed += new ElapsedEventHandler(cmdTimerF);
            // 设置引发时间的时间间隔 此处设置为１秒（１０００毫秒）
            phpTimer.Interval = 1000;
            phpTimer.Enabled = true;
            Console.WriteLine("php命令行合并文件服务启动成功");

            
        }

        #region 读取配置文件


        public static void ReadPHPMergeFile(string path) {
            try
            {
                StreamReader sr = new StreamReader(path, Encoding.Default);
                phpcmd = sr.ReadLine();
                Console.WriteLine("已成功配置php命令行执行地址");
            }
            catch (Exception e)
            {
                Console.WriteLine("未找到phpcmd.txt配置文件");
            }

        }
        /// <summary>
        /// 读取机器人图片文件夹地址
        /// </summary>
        /// <param name="path"></param>
        public static void ReadDirPath(string path)
        {

            try
            {
                StreamReader sr = new StreamReader(path, Encoding.Default);
                dirPath = sr.ReadLine();
                Console.WriteLine("已成功配置机器人图片地址");
            }
            catch (Exception e)
            {
                Console.WriteLine("未找到dirpath.txt配置文件");
            }

        }

        /// <summary>
        /// 读取动态ip解析的请求地址
        /// </summary>
        /// <param name="path"></param>
        public static void ReadDNSAPI(string path)
        {

            try
            {
                StreamReader sr = new StreamReader(path, Encoding.Default);
                apiUrl = sr.ReadLine();
                Console.WriteLine("已成功获取动态解析地址");
            }
            catch (Exception e)
            {
                Console.WriteLine("未找到dnsapi.txt配置文件");
            }

        }

        /// <summary>
        /// 读取图片的保存api地址
        /// </summary>
        /// <param name="path"></param>
        public static void ReadImageUrl(string path)
        {

            try
            {
                StreamReader sr = new StreamReader(path, Encoding.Default);
                imageUrl = sr.ReadLine();
                Console.WriteLine("已成功获取动态解析地址");
            }
            catch (Exception e)
            {
                Console.WriteLine("未找到dnsapi.txt配置文件");
            }

        }

        /// <summary>
        /// 读取需要拷贝的图片资源地址
        /// </summary>
        /// <param name="path"></param>
        public static void ReadImagePath(string path)
        {

            try
            {
                StreamReader sr = new StreamReader(path, Encoding.UTF8);
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    imagePathList.Add(line.ToString());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("未找到imagedirpath.txt配置文件");
            }

        }

        #endregion

        #region 定时器


        private static void cmdTimerF(object source, ElapsedEventArgs e)
        {

            try
            {
                phpTimer.Enabled = false;
                System.Diagnostics.Process p = new System.Diagnostics.Process();
                p.StartInfo.FileName = "cmd.exe";
                p.StartInfo.UseShellExecute = false;    //是否使用操作系统shell启动
                p.StartInfo.RedirectStandardInput = true;//接受来自调用程序的输入信息
                p.StartInfo.RedirectStandardOutput = true;//由调用程序获取输出信息
                p.StartInfo.RedirectStandardError = true;//重定向标准错误输出
                p.StartInfo.CreateNoWindow = true;//不显示程序窗口
                p.Start();//启动程序
                string str = phpcmd;
                p.StandardInput.WriteLine(str + "&exit");
                p.StandardInput.AutoFlush = true;
                //p.StandardInput.WriteLine("exit");
                //向标准输入写入要执行的命令。这里使用&是批处理命令的符号，表示前面一个命令不管是否执行成功都执行后面(exit)命令，如果不执行exit命令，后面调用ReadToEnd()方法会假死
                //同类的符号还有&&和||前者表示必须前一个命令执行成功才会执行后面的命令，后者表示必须前一个命令执行失败才会执行后面的命令

                //获取cmd窗口的输出信息
                string output = p.StandardOutput.ReadToEnd();
                p.WaitForExit();//等待程序执行完退出进程
                p.Close();
                Console.WriteLine(output);
                phpTimer.Enabled = true;
            }
            catch(Exception ex)
            {
                phpTimer.Enabled = true;
                Console.WriteLine(ex.Message);
                Console.WriteLine("########################################php合并文件命令异常########################################");
            }

           
        }




        /// <summary>
        /// 动态解析ip定时器
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private static void imageTimerF(object source, ElapsedEventArgs e)
        {
            jishu++;
            try
            {
                imageTimer.Enabled = false;
                foreach (string dir in imagePathList)
                {
                    //获取文件夹信息
                    DirectoryInfo imageDir = new DirectoryInfo(dir);
                    if (imageDir.Exists)
                    {
                        //查找文件
                        FileSystemInfo[] jpgFiles = imageDir.GetFileSystemInfos("*.jpg");
                        getImageList(jpgFiles, imageDir.FullName);
                        FileSystemInfo[] pngFiles = imageDir.GetFileSystemInfos("*.png");
                        getImageList(pngFiles, imageDir.FullName);
                        FileSystemInfo[] gifFiles = imageDir.GetFileSystemInfos("*.gif");
                        getImageList(gifFiles, imageDir.FullName);
                    }
                    else
                    {
                        Console.WriteLine("该文件夹不存在！" + imageDir.FullName);
                    }
                }
                foreach (string image in imageList)
                {
                    //获取图片信息
                    FileInfo file = new FileInfo(image);
                    string fileMD5Name = GetMD5HashFromFile(file.FullName);
                    string ext = file.Extension;
                    string res = HttpRequestHelper.HttpGet(imageUrl, "image=" + fileMD5Name + ext);
                    if ("添加成功".Equals(res))
                    {
                        CopyFile(file.FullName, dirPath);
                        string iamgePath = dirPath + file.Name;
                        string changeImagePath = dirPath + fileMD5Name + ext;
                        File.Move(iamgePath, changeImagePath);
                    }
                }
                imageList.Clear();
                imageTimer.Enabled = true;
                Console.WriteLine("=========================图片刷新成功一次" + jishu.ToString() + "=====================================");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("******************************图片刷新器代码异常**************************************");
                imageTimer.Enabled = true;
            }

            //string image ="";


        }

        private static void changeDNSTimer(object source, ElapsedEventArgs e)
        {
            jishu++;
            try
            {
                DnsTimer.Enabled = false;
                string res = webPost(apiUrl, "");
                Console.WriteLine(res);
                Console.WriteLine("+++++++++++++++++++++++++++++动态解析成功一次" + jishu.ToString() + "+++++++++++++++++++++++++++++++++++");
                DnsTimer.Enabled = true;
            }
            catch (Exception ex)
            {
                DnsTimer.Enabled = true;
                Console.WriteLine(ex.Message);
                Console.WriteLine("########################################动态解析代码异常########################################");
            }


        }

        private static string webPost(string url, string obj = "")
        {
            try
            {
                string param = (obj);//参数
                byte[] bs = Encoding.Default.GetBytes(param);
                HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
                req.Method = "POST";
                req.ContentType = "application/json";
                req.ContentLength = bs.Length;
                using (Stream reqStream = req.GetRequestStream())
                {
                    reqStream.Write(bs, 0, bs.Length);
                    reqStream.Close();
                    HttpWebResponse response2 = (HttpWebResponse)req.GetResponse();
                    StreamReader sr2 = new StreamReader(response2.GetResponseStream(), Encoding.UTF8);
                    string result = sr2.ReadToEnd();
                    return result;
                }
            }
            catch (Exception e)
            {
                return "";
            }

        }




        #endregion

        private static void getImageList(FileSystemInfo[] files, string dir)
        {
            foreach (FileSystemInfo file in files)
            {

                imageList.Add(file.FullName);
                //File.Move(file.FullName, dir + fileMD5Name + ext);
                // CopyFile(file.FullName, dirPath);
                //Console.WriteLine("已复制文件【{0}】", file.Name);

            }
        }

        public static string GetMD5HashFromFile(string filename)
        {
            try
            {
                FileStream file = new FileStream(filename, FileMode.Open);
                System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(file);
                file.Close();
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }

                return sb.ToString();
            }
            catch (System.Exception ex)
            {
                return null;
            }

        }

        private static void CopyFile(string srcFile, string destDir)
        {
            DirectoryInfo destDirectory = new DirectoryInfo(destDir);
            string fileName = Path.GetFileName(srcFile);
            if (!File.Exists(srcFile))
            {
                return;
            }

            if (!destDirectory.Exists)
            {
                destDirectory.Create();
            }

            File.Copy(srcFile, destDirectory.FullName + @"\" + fileName, true);

        }



        //private static void TimeEvent(object source, ElapsedEventArgs e)
        //{
        //    aTimer.Close();
        //    string str = Directory.GetCurrentDirectory()+ "\\..\\qqRobot\\酷Q Air\\data\\image";
        //    DirectoryInfo dir = new DirectoryInfo(str);
        //    if (dir.Exists)
        //    {
        //        //查找文件
        //        FileSystemInfo[] jpgFiles = dir.GetFileSystemInfos("*.jpg");
        //        foreachFile(jpgFiles);
        //        FileSystemInfo[] pngFiles = dir.GetFileSystemInfos("*.png");
        //        foreachFile(pngFiles);
        //        FileSystemInfo[] gifFiles = dir.GetFileSystemInfos("*.gif");
        //        foreachFile(gifFiles);
        //        FileSystemInfo[] tempFiles = dir.GetFileSystemInfos("*.cqimg");
        //        deleteTempFile(tempFiles);
        //    }
        //    else
        //    {
        //        Console.WriteLine("未找到该目录，你确定运行程序文件夹同级下是否有该目录【\\qqRobot\\酷Q Air\\data\\image】");
        //    }
        //    aTimer.Start();

        //}

        //private static void foreachFile(FileSystemInfo[] files) {
        //    foreach (FileSystemInfo file in files)
        //    {
        //        CopyFile(file.FullName, "D:/资源/图片/二次元/手机qq保存图片/");
        //        Console.WriteLine("已复制文件【{0}】", file.Name);
        //        file.Delete();
        //    }
        //}

        //private static void deleteTempFile(FileSystemInfo[] files) {
        //    foreach (FileSystemInfo file in files)
        //    {
        //        Console.WriteLine("已删除缓存文件【{0}】", file.Name);
        //        file.Delete();
        //    }
        //}


    }
}
