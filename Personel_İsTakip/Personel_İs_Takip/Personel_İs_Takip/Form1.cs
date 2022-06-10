using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
namespace Personel_İs_Takip
{
    public partial class Form1 : Form
    {
        SqlConnection baglanti = new SqlConnection("Server=DESKTOP-N1M8HFM ; Initial Catalog=PersonelTakip ;Integrated Security=SSPI");
        SqlCommand komut;
        SqlDataAdapter verial;
        DataSet ds;


     
        public Form1()
        {
            InitializeComponent();
           
        }

        public void Listele()
        {
            try
            {
                baglanti.Open();
                string sorgu = String.Format(@"SELECT P.*,PT.TurAdi FROM Personel P 
                          INNER JOIN PersonelTur PT ON P.PersonelTurID = PT.PersonelTurID
                          ORDER BY PersonelID DESC");
                verial = new SqlDataAdapter(sorgu, baglanti);
                ds = new DataSet();
                verial.Fill(ds); //gelen verileri dataset içine doldurma
                dataGridView1.DataSource = ds.Tables[0];
                baglanti.Close();
            }
            catch
            {
                MessageBox.Show("Personel listesi getirilirken bir hata meydana geldi!");
            }
        }
        private void btn_kaydet_Click(object sender, EventArgs e)
        {
            string personelKayitSorgu = string.Format(@"
                INSERT INTO [dbo]. [Personel]([AdiSoyadi],[Email],[Cinsiyeti],[PersonelTurID])
                     VALUES(@AdiSoyadi,@Email,@Cinsiyeti, @PersonelTurID)
                ");
            komut = new SqlCommand(personelKayitSorgu, baglanti);
            komut.Parameters.AddWithValue("@AdiSoyadi", txt_ad.Text);
            komut.Parameters.AddWithValue("@Email", txt_email.Text);
            komut.Parameters.AddWithValue("@Cinsiyeti", cmb_cinsiyet.SelectedItem);
            komut.Parameters.AddWithValue("@PersonelTurID", ((System.Data.DataRowView)cmb_personelturid.SelectedItem).Row.ItemArray[0]);

            try
            {
                baglanti.Open();
                komut.ExecuteNonQuery();
                baglanti.Close();
                MessageBox.Show("Yeni Personel Eklendi");
            }
            catch
            {
                MessageBox.Show("Yeni Personel Eklerken Bir Hata Meydana Geldi!");
            }
            Listele();




        }

        private void btn_listele_Click(object sender, EventArgs e)
        {
            Listele();

        }

        private void btn_temizle_Click(object sender, EventArgs e)
        {
            txt_ad.Clear();
            txt_email.Clear();
            cmb_cinsiyet.Text = " ";
            cmb_personelturid.Text = " ";


        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            lbl_personelid.Text = dataGridView1.CurrentRow.Cells[0].Value.ToString();
            txt_ad.Text = dataGridView1.CurrentRow.Cells[1].Value.ToString();
            txt_email.Text = dataGridView1.CurrentRow.Cells[2].Value.ToString();
            cmb_cinsiyet.Text = dataGridView1.CurrentRow.Cells[3].Value.ToString();
            cmb_personelturid.SelectedValue = dataGridView1.CurrentRow.Cells[4].Value.ToString();

        }

        private void btn_guncelle_Click(object sender, EventArgs e)
        {
            if (lbl_personelid.Text == "000000") MessageBox.Show("Lütfen personel seçiniz!");
            else
            {
                string personelUpdate = string.Format(@"UPDATE [dbo].[Personel]
                                                       SET[AdiSoyadi] = @AdiSoyadi
                                                          ,[Email] = @Email
                                                          ,[Cinsiyeti] = @Cinsiyeti
                                                          ,[PersonelTurID] = @PersonelTurID
                                                      Where PersonelID = @PersonelID");
                komut = new SqlCommand(personelUpdate, baglanti);
                komut.Parameters.AddWithValue("@AdiSoyadi", txt_ad.Text);
                komut.Parameters.AddWithValue("@Email", txt_email.Text);
                komut.Parameters.AddWithValue("@Cinsiyeti", cmb_cinsiyet.SelectedItem);
                komut.Parameters.AddWithValue("@PersonelTurID", ((System.Data.DataRowView)cmb_personelturid.SelectedItem).Row.ItemArray[0]);
                komut.Parameters.AddWithValue("@PersonelID", Convert.ToInt16(lbl_personelid.Text));
                try
                {
                    baglanti.Open();
                    komut.ExecuteNonQuery();
                    baglanti.Close();


                }
                catch 
                {

                    MessageBox.Show("Hata oluştu");
                }

                MessageBox.Show("Güncelleme Tamamlandı");
                Listele();
            }
        }


        private void btn_sil_Click(object sender, EventArgs e)
        {
            if (lbl_personelid.Text == "000000") MessageBox.Show("Personel Seçiniz!");
            else
            {
                DialogResult tus = MessageBox.Show("Personel Takip Bilgileri Silinsin Mi?", "UYARI", MessageBoxButtons.YesNo);
            if (tus == DialogResult.Yes)
            {
                komut = new SqlCommand("delete from Personel Where PersonelID=@PersonelID ", baglanti);
                komut.Parameters.AddWithValue("@PersonelID", Convert.ToInt16(lbl_personelid.Text));

                try
                {
                    baglanti.Open();
                    komut.ExecuteNonQuery();
                    baglanti.Close();

                }
                catch { MessageBox.Show("Bağlantı Hatası!"); }

                MessageBox.Show("Personel Bilgisi Silindi...");
                Listele();
                }

            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'personelTurleri.PersonelTur' table. You can move, or remove it, as needed.
            this.personelTurTableAdapter.Fill(this.personelTurleri.PersonelTur);

        }

       

        

        private void cmb_adsoyad_SelectedIndexChanged(object sender, EventArgs e)
        {
            baglanti.Close();

            baglanti.Open();
            SqlDataAdapter verial2 = new SqlDataAdapter("Select isID,isAdi,Detay,Sure,DurumAdi from Personel p,isler i,isDurumları id where p.PersonelID=i.PersonelID and i.isDurumID=id.isDurumID and AdiSoyadi='" + cmb_adsoyad.Text + "'", baglanti);
            DataSet ds2 = new DataSet();
            verial2.Fill(ds2);
            dataGridView2.DataSource = ds2.Tables[0];
            baglanti.Close();
        }

        private void btn_kaydet2_Click(object sender, EventArgs e)
        {


            if (Convert.ToInt32(lblIsNumarasi.Text) > 0)
            {
                MessageBox.Show("Var olan kayıt tekrar eklenemez.");
            }
            else { 
            komut = new SqlCommand("INSERT INTO [dbo].[isler] ([isAdi] ,[Detay] ,[Sure] ,[isDurumID] ,[PersonelID]) VALUES (@isAdi,@Detay,@Sure,@isDurumID,@PersonelID)", baglanti);
            
            komut.Parameters.AddWithValue("@isAdi", txt_isadi.Text);
            komut.Parameters.AddWithValue("@Detay", txt_isdetay.Text);
            komut.Parameters.AddWithValue("@Sure", txt_sure.Text);
            komut.Parameters.AddWithValue("@isDurumID", ((System.Data.DataRowView)cmb_isdurumu.SelectedItem).Row.ItemArray[0]);
            komut.Parameters.AddWithValue("@PersonelID", ((System.Data.DataRowView)cmb_adsoyad.SelectedItem).Row.ItemArray[0]);
            baglanti.Open();

            komut.ExecuteNonQuery();
            baglanti.Close();

            MessageBox.Show("Yeni iş durumları tanımlandı");

            }
        }

        private void dataGridView2_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            lblIsNumarasi.Text = dataGridView2.CurrentRow.Cells[0].Value.ToString();
            txt_isadi.Text = dataGridView2.CurrentRow.Cells[1].Value.ToString();
            txt_isdetay.Text = dataGridView2.CurrentRow.Cells[2].Value.ToString();
            txt_sure.Text = dataGridView2.CurrentRow.Cells[3].Value.ToString();
            cmb_isdurumu.Text = dataGridView2.CurrentRow.Cells[4].Value.ToString();

        }

        private void btn_guncelle2_Click(object sender, EventArgs e)
        {
            
            if (cmb_adsoyad.SelectedItem == null) MessageBox.Show("Lütfen personel seçiniz!");
            else
            {

                komut = new SqlCommand("UPDATE [dbo].[isler] SET [isAdi] = @isAdi, [Detay] =@Detay, [Sure] =@Sure, [isDurumID]= @isDurumID, [PersonelID]= @PersonelID WHERE [PersonelID] = @PersonelID AND [isID] = @isID ", baglanti);
                komut.Parameters.AddWithValue("@isAdi", txt_isadi.Text);
                komut.Parameters.AddWithValue("@Detay", txt_isdetay.Text);
                komut.Parameters.AddWithValue("@Sure", txt_sure.Text);
                komut.Parameters.AddWithValue("@isDurumID", ((System.Data.DataRowView)cmb_isdurumu.SelectedItem).Row.ItemArray[0]);
                komut.Parameters.AddWithValue("@PersonelID", ((System.Data.DataRowView)cmb_adsoyad.SelectedItem).Row.ItemArray[0]);
                komut.Parameters.AddWithValue("@isID", lblIsNumarasi.Text);
                
                baglanti.Open();

                komut.ExecuteNonQuery();
                baglanti.Close();



            }
        }

        private void btn_temizle2_Click(object sender, EventArgs e)
        {
            txt_isadi.Clear();
            txt_isdetay.Clear();
            txt_sure.Clear();
            cmb_isdurumu.Text = " ";
            cmb_adsoyad.Text = " ";

        }

        private void Form1_Load_1(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'personellerDataSet.Personel' table. You can move, or remove it, as needed.
            this.personelTableAdapter.Fill(this.personellerDataSet.Personel);
            // TODO: This line of code loads data into the 'personelTakipDataSet2.isDurumları' table. You can move, or remove it, as needed.
            this.isDurumlarıTableAdapter.Fill(this.personelTakipDataSet2.isDurumları);
            // TODO: This line of code loads data into the 'personelTakipDataSet1.PersonelTur' table. You can move, or remove it, as needed.
            this.personelTurTableAdapter1.Fill(this.personelTakipDataSet1.PersonelTur);

        }

        private void button2_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPage2;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPage3;
        }

        private void cb_personeladi_CheckedChanged(object sender, EventArgs e)
        {
            if (cb_personeladi.Checked == true)
                txt_personel.Enabled = true;
            else
            {
                txt_personel.Enabled = false;
                txt_personel.Clear();
            }
        }

        private void cb_turadi_CheckedChanged(object sender, EventArgs e)
        {

            if (cb_turadi.Checked == true)
                cmb_tur.Enabled = true;
            else
            {
                cmb_tur.Enabled = false;
                cmb_tur.Text = "";

            }
        }

        private void cb_durumadi_CheckedChanged(object sender, EventArgs e)
        {

            if (cb_durumadi.Checked == true)
                cmb_durum.Enabled = true;
            else
            {
                cmb_durum.Enabled = false;
                cmb_durum.Text = "";

            }
        }

        private void fillByToolStripButton_Click(object sender, EventArgs e)
        {
            try
            {
                this.personelTurTableAdapter.FillBy(this.personelTurleri.PersonelTur);
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }

        }

        private void btn_detaylıarama_Click(object sender, EventArgs e)
        {
            baglanti.Close();

            baglanti.Open();


            string secilenkriterler = "";
            if (cb_personeladi.Checked == true) { secilenkriterler += "and AdiSoyadi like '%"+txt_personel.Text+"%'" ; }//isim kriteri ekle
             if (cb_turadi.Checked == true) { secilenkriterler += "and TurAdi ='" + cmb_tur.Text + "'"; } //tür kriteri ekle
            if (cb_durumadi.Checked == true) { secilenkriterler += "and DurumAdi ='" + cmb_durum.Text + "'"; }//tutar kriteri ekle

            //MessageBox.Show(secilenkriterler);

            SqlDataAdapter verial3 = new SqlDataAdapter("select AdiSoyadi,TurAdi,isAdi,DurumAdi from Personel p, PersonelTur pt," +
                " isler i, isDurumları id where pt.PersonelTurID = p.PersonelTurID and p.PersonelID = i.PersonelID " +
                "and i.isDurumID = id.isDurumID "+ secilenkriterler, baglanti);
            DataSet ds3 = new DataSet();
            verial3.Fill(ds3);
            dataGridView3.DataSource = ds3.Tables[0];
            baglanti.Close();
        }

        private void btnsil2_Click(object sender, EventArgs e)
        {
            if (lblIsNumarasi.Text == "000000") MessageBox.Show("İş Seçiniz!");
            else
            {
                DialogResult tus = MessageBox.Show("Personel İş  Bilgileri Silinsin Mi?", "UYARI", MessageBoxButtons.YesNo);
                if (tus == DialogResult.Yes)
                {
                    komut = new SqlCommand("delete from isler Where isID=@isID ", baglanti);
                    komut.Parameters.AddWithValue("@isID", Convert.ToInt16(lblIsNumarasi.Text));

                    try
                    {
                        baglanti.Open();
                        komut.ExecuteNonQuery();
                        baglanti.Close();

                    }
                    catch { MessageBox.Show("Bağlantı Hatası!"); }

                    MessageBox.Show("Personel İş  Bilgisi Silindi...");
                    
                }

            }
        }

       

        
    }
}
