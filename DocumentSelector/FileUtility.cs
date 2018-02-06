using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace DocumentSelector
{
    class FileUtility
    {
        public static void createFile(string filePath)
        {
            FileInfo fi = new FileInfo(filePath);
            if (!fi.Exists)
            {
                fi.Create();
            }
        }

        private static bool copyFile(string source, string dest, bool overwrite)
        {
            try
            {
                File.Copy(source, dest, overwrite);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("文件复制失败," + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public static bool moveFile(string source, string dest)
        {
            try
            {
                //File.Copy(source, dest, overwrite);
                File.Move(source, dest);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("文件移动失败," + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public static bool deleteFile(string path)
        {
            try
            {
                File.Delete(path);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("文件删除失败," + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }




        public static void createDirectory(string path)
        {
            try
            {
                DirectoryInfo di = new DirectoryInfo(path);
                if (!di.Exists)
                {
                    di.Create();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("目录创建失败," + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void deleteDirectory(string path)
        {
            try
            {
                DirectoryInfo di = new DirectoryInfo(path);
                if (di.Exists)
                {
                    di.Delete(true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("目录删除失败," + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void RenameDirectory(string old_name, string new_name)
        {
             try
            {
                DirectoryInfo di = new DirectoryInfo(old_name);
                if (di.Exists)
                {
                    Directory.Move(old_name, new_name);
                }
            }
             catch (Exception ex)
             {
                 MessageBox.Show("目录重命名失败," + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
             }
        }

        /// <summary>
        /// 遍历当前文件夹下的文件，不包括子文件夹
        /// </summary>
        /// <param name="info"></param>
        /// <param name="filter"></param>
        /// <param name="list"></param>
        public static void getFileUnderDirectory(FileSystemInfo info, string filter, List<Item> list)
        {
            if (!info.Exists)
            {
                return;
            }
            else
            {
                DirectoryInfo dirInfo = info as DirectoryInfo;
                if (dirInfo != null)
                {
                    foreach (var file in dirInfo.GetFileSystemInfos())
                    {
                        FileInfo fileInfo = file as FileInfo;
                        if (fileInfo != null)
                        {
                            string fileName = fileInfo.Name;
                            string filePath = fileInfo.FullName;
                            if (filter != null && filter.Trim().Length > 0)
                            {
                                if (fileName.IndexOf(filter) > -1)
                                {

                                    Item item = new Item(fileName, filePath);
                                    list.Add(item);
                                }
                            }
                            else
                            {
                                Item item = new Item(fileName, filePath);
                                list.Add(item);
                            }
                        }
                        
                    }
                }
            }
        }

        /// <summary>
        /// 遍历所有文件夹下的文件，包括子文件夹
        /// </summary>
        /// <param name="info"></param>
        /// <param name="filter"></param>
        /// <param name="list"></param>
        public static void getAllFileUnderDirectory(FileSystemInfo info, string filter, List<Item> list)
        {
         
            if (!info.Exists)
            {
                return;
            }
            else
            {
                DirectoryInfo dirInfo = info as DirectoryInfo;
                if (dirInfo != null)
                {
                    foreach (var file in dirInfo.GetFileSystemInfos())
                    {
                        FileInfo fileInfo = file as FileInfo;
                        if (fileInfo != null)
                        {
                            string fileName = fileInfo.Name;
                            string filePath = fileInfo.FullName;
                            if (filter != null && filter.Trim().Length > 0)
                            {
                                if (fileName.IndexOf(filter) > -1)
                                {

                                    Item item = new Item(fileName, filePath);
                                    list.Add(item);
                                }
                            }
                            else
                            {
                                Item item = new Item(fileName, filePath);
                                list.Add(item);
                            }
                        }
                        else
                        {
                            Application.DoEvents(); 
                            getAllFileUnderDirectory(file as DirectoryInfo, filter, list);
                        }
                    }
                }
            }
        }

       
        public static void CopyFile(string FormerFile, string toFile)
        {
            FileStream FormerOpen;
            FileStream ToFileOpen;
            int SectSize = 100 * 1024 * 1024;
            FileStream fileToCreate = new FileStream(toFile, FileMode.Create);		//创建目的文件，如果已存在将被覆盖
            fileToCreate.Close();										//关闭所有资源
            fileToCreate.Dispose();										//释放所有资源
            FormerOpen = new FileStream(FormerFile, FileMode.Open, FileAccess.Read);//以只读方式打开源文件
            ToFileOpen = new FileStream(toFile, FileMode.Append, FileAccess.Write);	//以写方式打开目的文件
            //根据一次传输的大小，计算传输的个数
            //int max = Convert.ToInt32(Math.Ceiling((double)FormerOpen.Length / (double)SectSize));

            int FileSize;												//要拷贝的文件的大小
            //如果分段拷贝，即每次拷贝内容小于文件总长度
            if (SectSize < FormerOpen.Length)
            {
                byte[] buffer = new byte[SectSize];							//根据传输的大小，定义一个字节数组
                int copied = 0;										//记录传输的大小
                while (copied <= ((int)FormerOpen.Length - SectSize))			//拷贝主体部分
                {
                    FileSize = FormerOpen.Read(buffer, 0, SectSize);			//从0开始读，每次最大读SectSize
                    FormerOpen.Flush();								//清空缓存
                    ToFileOpen.Write(buffer, 0, SectSize);					//向目的文件写入字节
                    ToFileOpen.Flush();									//清空缓存
                    ToFileOpen.Position = FormerOpen.Position;				//使源文件和目的文件流的位置相同
                    copied += FileSize;									//记录已拷贝的大小
                }
                int left = (int)FormerOpen.Length - copied;						//获取剩余大小
                FileSize = FormerOpen.Read(buffer, 0, left);					//读取剩余的字节
                FormerOpen.Flush();									//清空缓存
                ToFileOpen.Write(buffer, 0, left);							//写入剩余的部分
                ToFileOpen.Flush();									//清空缓存
            }
            //如果整体拷贝，即每次拷贝内容大于文件总长度
            else
            {
                byte[] buffer = new byte[FormerOpen.Length];				//获取文件的大小
                FormerOpen.Read(buffer, 0, (int)FormerOpen.Length);			//读取源文件的字节
                FormerOpen.Flush();									//清空缓存
                ToFileOpen.Write(buffer, 0, (int)FormerOpen.Length);			//写放字节
                ToFileOpen.Flush();									//清空缓存
            }
            FormerOpen.Close();										//释放所有资源
            ToFileOpen.Close();										//释放所有资源
        }

    }
}
