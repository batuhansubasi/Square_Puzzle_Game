//Emre Altay 160202069
//Batuhan Subasi 160202091 

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace WindowsFormsApplication2
{
    public partial class Puzzle : Form
    {
        //Data definitions....
        String dosya_ismi;
        Image resim;
        Image[] resimler = new Image[17];
        String[] resimlerbase64 = new String[17];
        String[] butonbase64 = new String[17];
        Button karistir = new Button();
        Button[] buttonArray = new Button[17];
        Label skor = new Label();
        Label en_yuksek_skor = new Label();
        int puan = 100;
        int global_count = 0;
        int secilen = 0;
        int click_count = 0;
        int en_yuksek;
        int cs = 0;
        Point newLoc = new Point(0, 0);

        public Puzzle()
        {
            InitializeComponent();
            en_yuksek_skor_bulma();
        }

        public void en_yuksek_skor_bulma()
        {
            string path = @" C:\Users\Batuhan.subasi\Desktop\enyüksekskor.txt";
            if (File.Exists(path))
            {
                string[] lines = File.ReadLines(path).ToArray();
                int[] nums = new int[lines.Length];
                for (int i = 0; i < lines.Length; i++)
                {
                    nums[i] = int.Parse(lines[i]);
                }
                en_yuksek = nums.Max();
            }
            else
            {
                en_yuksek = 0;
            }
        }

        public Boolean resimOku()   //Resmin secilmesi...
        {
            OpenFileDialog files = new OpenFileDialog();
            files.Title = "Resim Seçim";
            files.Filter = "Resim Dosyalari (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
            files.ShowDialog();

            if (string.IsNullOrEmpty(files.FileName))
            {
                return false;
            }

            dosya_ismi = files.FileName;

            return true;
        }

        public void dizayn(int i)   //Secilen Resmin ayirt edilebilmesi icin boyanmasi...
        {
            click_count++;

            if (click_count == 1) //ilk secilen
            {
                buttonArray[i].FlatStyle = FlatStyle.Flat;
                buttonArray[i].FlatAppearance.BorderColor = Color.Red;
                buttonArray[i].FlatAppearance.BorderSize = 2;
                secilen = i;
            }

            if (click_count == 2) //ikinci secilen
            {
                puan = puan - 1;
                skor.Text = "Anlik Skor:" + puan;

                Image temp = buttonArray[i].BackgroundImage;
                buttonArray[i].BackgroundImage = buttonArray[secilen].BackgroundImage;
                buttonArray[secilen].BackgroundImage = temp;

                for (int j = 1; j < 17; j++)
                {
                    buttonArray[j].FlatStyle = FlatStyle.Flat;
                    buttonArray[j].FlatAppearance.BorderColor = SystemColors.Control;
                    buttonArray[j].FlatAppearance.BorderSize = 2;
                }

                if (tamamlandiKontrol() == true || puan < 0)
                {
                    txtdosyasidoldur();
                    MessageBox.Show("Tebrikler, oyunu bitirdiniz... Skorunuz:" + puan.ToString());
                    Application.Exit();
                    System.Windows.Forms.Application.Exit();
                    System.Environment.Exit(1);
                }
                click_count = 0;
            }
        }

        private static Image Crop(Image image, Rectangle rect)  //Ekrana Sigdirma Crop
        {
            Bitmap resim = new Bitmap(image);
            Bitmap parcaResim = resim.Clone(rect, resim.PixelFormat);

            return (Image)(parcaResim);
        }

        public void ilkSenaryo()    //Baslatma
        {
            int x = resim.Width / 4;
            int y = resim.Height / 4;
            karistir.Text = "Karistir";
            int sayac = 1;
            Rectangle cropAlani = new Rectangle(0, 0, x, y);

            for (int d = 0; d < 4; d++)
            {
                for (int c = 0; c < 4; c++)
                {
                    cropAlani.Y = d * y;
                    cropAlani.X = c * x;
                    Image parcaResim = Crop(resim, cropAlani);
                    buttonArray[sayac].BackgroundImage = parcaResim;
                    resimler[sayac] = parcaResim;
                    buttonArray[sayac].Visible = false;
                    sayac++;
                }
            }
        }

        public void klasikSenaryo() //Karistirma...
        {
            int x = resim.Width / 4;
            int y = resim.Height / 4;
            Random rastgele = new Random();
            ArrayList liste = new ArrayList();
            Rectangle cropAlani = new Rectangle(0, 0, x, y);

            for (int d = 0; d < 4; d++)
            {
                for (int c = 0; c < 4; c++)
                {

                    int sayac = rastgele.Next(1, 17); //random sayi ürettik.

                    Found:
                    for (int i = 0; i < liste.Count; i++)
                    {
                        if (Convert.ToInt32(liste[i]) == sayac)
                        {
                            if (liste.Count != 16)
                            {
                                sayac = rastgele.Next(1, 17);
                                goto Found;
                            }
                        }
                    }

                    liste.Add(sayac);
                    cropAlani.Y = d * y;
                    cropAlani.X = c * x;
                    Image parcaResim = Crop(resim, cropAlani);
                    buttonArray[sayac].BackgroundImage = parcaResim;
                    buttonArray[sayac].Visible = true;
                }
            }

            int sayac5 = 0;
            for (int i = 1; i < 17; i++)
            {
                var stream = new MemoryStream();
                resimler[i].Save(stream, ImageFormat.Jpeg);
                var bytes = stream.ToArray();
                resimlerbase64[i] = Convert.ToBase64String(bytes);

                stream = new MemoryStream();
                buttonArray[i].BackgroundImage.Save(stream, ImageFormat.Jpeg);
                bytes = stream.ToArray();
                butonbase64[i] = Convert.ToBase64String(bytes);

                if (resimlerbase64[i].Equals(butonbase64[i]))
                {
                    sayac5++;
                }
            }

            if (sayac5 == 1)
            {
                MessageBox.Show(sayac5.ToString() + " parca dogru yerdedir, artik karistirma yapamazsiniz.");
                karistir.Visible = false;
            }
            else if (sayac5 == 16 || puan < 0)
            {
                MessageBox.Show("Tebrikler, oyunu bitirdiniz... Skorunuz:" + puan.ToString());
                Application.Exit();
                System.Windows.Forms.Application.Exit();
                System.Environment.Exit(1);
            }
            puan = puan - 5;
            skor.Text = "Anlik Skor:" + puan;
            skor.Font = new Font("Arial", 15);

        }

        public static Image resizeImage(Image imgToResize, Size size)   //Resize
        {
            return (Image)(new Bitmap(imgToResize, size));
        }

        public void start() //Resim Secildikten Sonra Kontroller...
        {
            MessageBox.Show("Resim seçme ekrani geliyor", "Puzzle");
            Boolean kontrol = resimOku();

            while (kontrol == false)
            {
                DialogResult result = MessageBox.Show("Seçim Yapmadiniz! Çikis yapmak istiyor musunuz?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                if (result != DialogResult.Yes)
                {
                    kontrol = resimOku();
                }
                else
                {
                    kontrol = true;
                    Application.Exit();
                    System.Windows.Forms.Application.Exit();
                    System.Environment.Exit(1);
                }
            }

            resim = Image.FromFile(dosya_ismi);
            if (resim.Width % 4 != 0 || resim.Height % 4 != 0)
            {
                MessageBox.Show("Seçtiginiz Resim 16 puzzle karesine bölünemiyor!!!", "Error");
                Application.Exit();
            }

            if (resim.Width > 1000 || resim.Height > 600)
            {
                double tempx = resim.Width;
                double tempy = resim.Height;

                while (tempx > 1000 || tempy > 600)
                {
                    tempx = tempx / 2;
                    tempy = tempy / 2;
                    tempx = tempx * 1.75;
                    tempy = tempy * 1.75;
                }
                resim = resizeImage(resim, new Size(Convert.ToInt32(tempx), Convert.ToInt32(tempy)));
            }

        }

        private void Form1_Load(object sender, EventArgs e) //Formun Yüklenmesi, Main....
        {
            start();
            this.Size = new Size(resim.Width + 21, resim.Height + 100);

            int x = resim.Width / 4;
            int y = resim.Height / 4;

            for (int i = 1; i < 17; i++)
            {
                switch (i)
                {
                    case 2:
                    case 3:
                    case 4:
                    case 6:
                    case 7:
                    case 8:
                    case 10:
                    case 11:
                    case 12:
                    case 14:
                    case 15:
                    case 16:
                        newLoc.Offset(x + 1, 0);
                        break;
                    case 5:
                    case 9:
                    case 13:
                        newLoc.Offset((((x + 1) * 3) * (-1)), y + 1);
                        break;
                }
                buttonArray[i] = new Button();
                buttonArray[i].Size = new Size(x, y);
                buttonArray[i].Location = newLoc;
                buttonArray[i].Name = i.ToString();
                buttonArray[i].FlatStyle = FlatStyle.Flat;
                buttonArray[i].FlatAppearance.BorderColor = Color.White;
                buttonArray[i].FlatAppearance.BorderSize = 2;
                this.Controls.Add(buttonArray[i]);
            }

            newLoc.Offset((((x + 1) * 3) * (-1)), y + 1);
            skor.Size = new Size(x, 70);
            skor.Location = newLoc;
            skor.Text = "Anlik Skor:" + puan + Environment.NewLine +
                "NOT: Karistir(-5 Puan) - Resim Degistir(-1 Puan)";
            skor.Font = new Font("Arial", 10);
            this.Controls.Add(skor);

            newLoc.Offset(x + 1, 0);
            karistir.Size = new Size(2 * x, 50);
            karistir.Location = newLoc;
            karistir.Text = "Basla";
            karistir.Font = new Font("Arial", 15);
            this.Controls.Add(karistir);

            newLoc.Offset(2 * x, 0);
            en_yuksek_skor.Size = new Size(x, 50);
            en_yuksek_skor.Location = newLoc;
            en_yuksek_skor.Text = "En Yuksek Skor:" + en_yuksek;
            en_yuksek_skor.Font = new Font("Arial", 15);
            this.Controls.Add(en_yuksek_skor);

            karistir.Click += karistir_Click;

            for (int i = 1; i < 17; i++)
            {
                int index = i;
                buttonArray[i].Click += new EventHandler(common_Click);
            }

            String photospath = @"C:\Users\Batuhan.subasi\Desktop\photos";
            if (File.Exists(photospath))
            {
                File.Delete(photospath);
            }
            else
            {
                System.IO.Directory.CreateDirectory(photospath);
            }

        }

        void common_Click(object sender, EventArgs e) //Buttonlara tiklanilmasi...
        {
            if (karistir.Text.Equals("Karistir"))
            {
                switch (((Button)(sender)).Name.ToString())
                {
                    case "1":
                        dizayn(1);
                        break;
                    case "2":
                        dizayn(2);
                        break;
                    case "3":
                        dizayn(3);
                        break;
                    case "4":
                        dizayn(4);
                        break;
                    case "5":
                        dizayn(5);
                        break;
                    case "6":
                        dizayn(6);
                        break;
                    case "7":
                        dizayn(7);
                        break;
                    case "8":
                        dizayn(8);
                        break;
                    case "9":
                        dizayn(9);
                        break;
                    case "10":
                        dizayn(10);
                        break;
                    case "11":
                        dizayn(11);
                        break;
                    case "12":
                        dizayn(12);
                        break;
                    case "13":
                        dizayn(13);
                        break;
                    case "14":
                        dizayn(14);
                        break;
                    case "15":
                        dizayn(15);
                        break;
                    case "16":
                        dizayn(16);
                        break;
                }
            }
        }

        private void karistir_Click(object sender, EventArgs e) //Baslat, karistir butonuna basildiginda...
        {

            if (global_count == 0)
            {
                ilkSenaryo();
            }

            else
            {
                klasikSenaryo();
            }
            global_count++;
        }

        public void txtdosyasidoldur()
        {
            string path = @" C:\Users\Batuhan.subasi\Desktop\enyüksekskor.txt";
            if (!File.Exists(path))
            {
                File.Create(path).Close();
                TextWriter tw = new StreamWriter(path);
                tw.WriteLine(puan);
                tw.Dispose();
            }
            else if (File.Exists(path))
            {
                using (var tw = new StreamWriter(path, true))
                {
                    tw.WriteLine(puan);
                }
            }
        }

        public Boolean tamamlandiKontrol()
        {
            int width = resim.Width;
            int height = resim.Height;
            cs++;
            String jpg3 = @"C:\Users\Batuhan.subasi\Desktop\photos\resim" + cs + ".jpg";
            String resimstring, orjinal_resim_string;

            Bitmap img = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(img);

            g.Clear(Color.Black);
            g.DrawImage(buttonArray[1].BackgroundImage, new Point(0, 0));
            g.DrawImage(buttonArray[2].BackgroundImage, new Point(buttonArray[1].Width, 0));
            g.DrawImage(buttonArray[3].BackgroundImage, new Point(buttonArray[1].Width * 2, 0));
            g.DrawImage(buttonArray[4].BackgroundImage, new Point(buttonArray[1].Width * 3, 0));

            g.DrawImage(buttonArray[5].BackgroundImage, new Point(0, buttonArray[1].Height));
            g.DrawImage(buttonArray[6].BackgroundImage, new Point(buttonArray[1].Width, buttonArray[1].Height));
            g.DrawImage(buttonArray[7].BackgroundImage, new Point(buttonArray[1].Width * 2, buttonArray[1].Height));
            g.DrawImage(buttonArray[8].BackgroundImage, new Point(buttonArray[1].Width * 3, buttonArray[1].Height));

            g.DrawImage(buttonArray[9].BackgroundImage, new Point(0, buttonArray[1].Height * 2));
            g.DrawImage(buttonArray[10].BackgroundImage, new Point(buttonArray[1].Width, buttonArray[1].Height * 2));
            g.DrawImage(buttonArray[11].BackgroundImage, new Point(buttonArray[1].Width * 2, buttonArray[1].Height * 2));
            g.DrawImage(buttonArray[12].BackgroundImage, new Point(buttonArray[1].Width * 3, buttonArray[1].Height * 2));

            g.DrawImage(buttonArray[13].BackgroundImage, new Point(0, buttonArray[1].Height * 3));
            g.DrawImage(buttonArray[14].BackgroundImage, new Point(buttonArray[1].Width, buttonArray[1].Height * 3));
            g.DrawImage(buttonArray[15].BackgroundImage, new Point(buttonArray[1].Width * 2, buttonArray[1].Height * 3));
            g.DrawImage(buttonArray[16].BackgroundImage, new Point(buttonArray[1].Width * 3, buttonArray[1].Height * 3));

            g.Dispose();
            img.Save(jpg3, ImageFormat.Png);
            img.Dispose();

            Image imgz = Image.FromFile(jpg3);
            var stream = new MemoryStream();
            imgz.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
            var bytes = stream.ToArray();
            resimstring = Convert.ToBase64String(bytes);

            stream = new MemoryStream();
            resim.Save(stream, ImageFormat.Jpeg);
            bytes = stream.ToArray();
            orjinal_resim_string = Convert.ToBase64String(bytes);

            if (resimstring.Equals(orjinal_resim_string))
            {
                return true;
            }
            else
            {
                int sayac5 = 0;
                for (int i = 1; i < 17; i++)
                {
                    stream = new MemoryStream();
                    resimler[i].Save(stream, ImageFormat.Jpeg);
                    bytes = stream.ToArray();
                    resimlerbase64[i] = Convert.ToBase64String(bytes);

                    stream = new MemoryStream();
                    buttonArray[i].BackgroundImage.Save(stream, ImageFormat.Jpeg);
                    bytes = stream.ToArray();
                    butonbase64[i] = Convert.ToBase64String(bytes);

                    if (resimlerbase64[i].Equals(butonbase64[i]))
                    {
                        sayac5++;
                    }
                }

                if (sayac5 == 16) return true;
                else return false;
            }

        }


    }
}
