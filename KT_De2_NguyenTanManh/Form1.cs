using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KT_De2_NguyenTanManh.Models;


namespace KT_De2_NguyenTanManh
{
    //private bool them();
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                // Khởi tạo context để kết nối với cơ sở dữ liệu
                Model1 context = new Model1();

                // Lấy danh sách sản phẩm từ cơ sở dữ liệu
                List<SANPHAM> listSanPhams = context.SANPHAMs.ToList();

                // Đổ dữ liệu vào DataGridView
                BindGrid(listSanPhams);

                // Gán giá trị cho DateTimePicker (nếu có sản phẩm)
                if (listSanPhams.Count > 0)
                {
                    var ngayNhap = listSanPhams.First().NgayNhap;

                    if (ngayNhap.HasValue)
                    {
                        dtpNgayNhap.Value = ngayNhap.Value; // Gán giá trị ngày nhập đầu tiên
                    }
                    else
                    {
                        dtpNgayNhap.Value = DateTime.Today; // Gán ngày mặc định nếu không có giá trị
                        MessageBox.Show("Ngày nhập của sản phẩm đầu tiên không hợp lệ. Đã đặt về ngày hiện tại.");
                    }
                }
                else
                {
                    dtpNgayNhap.Value = DateTime.Today; // Không có sản phẩm, đặt mặc định ngày hiện tại
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
            btnLuu.Enabled = false;
            btnKLuu.Enabled = false;
        }

       
        private void BindGrid(List<SANPHAM> listSanPhams)
        {
            dgvQLSP.Rows.Clear(); // Xóa các dòng cũ

            foreach (var sp in listSanPhams)
            {
                dgvQLSP.Rows.Add(
                    sp.MaSP,
                    sp.TenSP,
                    sp.NgayNhap.HasValue ? sp.NgayNhap.Value.ToString("dd/MM/yyyy") : "N/A",
                    sp.MaLoai
                );
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            btnLuu.Enabled = true;
            btnKLuu.Enabled = true;
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            try
            {
                // Khởi tạo context để kết nối với cơ sở dữ liệu
                Model1 context = new Model1();

                // Lấy thông tin từ giao diện
                string maSP = txtMaSP.Text.Trim();
                string tenSP = txtTenSP.Text.Trim();
                DateTime ngayNhap = dtpNgayNhap.Value;
                string maLoai = txtLoaiSP.Text.Trim();

                // Kiểm tra dữ liệu nhập vào
                if (string.IsNullOrEmpty(maSP) || string.IsNullOrEmpty(tenSP) || string.IsNullOrEmpty(maLoai))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Kiểm tra trùng mã sản phẩm
                if (context.SANPHAMs.Any(sp => sp.MaSP == maSP))
                {
                    MessageBox.Show("Mã sản phẩm đã tồn tại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Tạo đối tượng SANPHAM mới
                SANPHAM newSanPham = new SANPHAM
                {
                    MaSP = maSP,
                    TenSP = tenSP,
                    NgayNhap = ngayNhap,
                    MaLoai = maLoai
                };

                // Thêm vào DbSet và lưu thay đổi
                context.SANPHAMs.Add(newSanPham);
                context.SaveChanges();

                // Thông báo thành công
                MessageBox.Show("Thêm sản phẩm thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Cập nhật DataGridView
                List<SANPHAM> listSanPhams = context.SANPHAMs.ToList();
                BindGrid(listSanPhams);

                // Xóa nội dung nhập trên giao diện
                ClearInputFields();
                btnLuu.Enabled = true;
                btnKLuu.Enabled = true;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            try
            {
                // Khởi tạo context để kết nối với cơ sở dữ liệu
                Model1 context = new Model1();

                // Lấy thông tin từ giao diện
                string maSP = txtMaSP.Text.Trim();
                string tenSP = txtTenSP.Text.Trim();
                DateTime ngayNhap = dtpNgayNhap.Value;
                string maLoai = txtLoaiSP.Text.Trim();

                // Kiểm tra dữ liệu nhập vào
                if (string.IsNullOrEmpty(maSP) || string.IsNullOrEmpty(tenSP) || string.IsNullOrEmpty(maLoai))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Tìm sản phẩm trong cơ sở dữ liệu theo mã sản phẩm
                SANPHAM existingSanPham = context.SANPHAMs.FirstOrDefault(sp => sp.MaSP == maSP);

                if (existingSanPham == null)
                {
                    MessageBox.Show("Sản phẩm không tồn tại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Cập nhật thông tin sản phẩm
                existingSanPham.TenSP = tenSP;
                existingSanPham.NgayNhap = ngayNhap;
                existingSanPham.MaLoai = maLoai;

                // Lưu thay đổi vào cơ sở dữ liệu
                context.SaveChanges();

                // Thông báo thành công
                MessageBox.Show("Sửa sản phẩm thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Cập nhật lại DataGridView
                List<SANPHAM> listSanPhams = context.SANPHAMs.ToList();
                BindGrid(listSanPhams);

                // Xóa nội dung nhập trên giao diện
                ClearInputFields();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnKLuu_Click(object sender, EventArgs e)
        {
            try
            {
                
                ClearInputFields();

                
                btnLuu.Enabled = false;
                btnKLuu.Enabled = false;

                
                using (Model1 context = new Model1())
                {
                    List<SANPHAM> listSanPhams = context.SANPHAMs.ToList();
                    BindGrid(listSanPhams);  // Rebind the DataGridView to the updated list
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi hủy thay đổi: " + ex.Message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void ClearInputFields()
        {
            txtMaSP.Clear();
            txtTenSP.Clear();
            txtLoaiSP.Clear();
            dtpNgayNhap.Value = DateTime.Today; // Đặt ngày mặc định là hôm nay
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            btnLuu.Enabled = true;
            btnKLuu.Enabled = true;
        }

        private void dgvQLSP_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                // Kiểm tra xem dòng được nhấn có hợp lệ không (bỏ qua tiêu đề)
                if (e.RowIndex >= 0 && e.RowIndex < dgvQLSP.Rows.Count)
                {
                    // Lấy dòng được nhấn
                    DataGridViewRow selectedRow = dgvQLSP.Rows[e.RowIndex];

                    // Gán giá trị từ dòng được nhấn vào các ô nhập liệu
                    txtMaSP.Text = selectedRow.Cells[0].Value?.ToString();
                    txtTenSP.Text = selectedRow.Cells[1].Value?.ToString();
                    dtpNgayNhap.Text = selectedRow.Cells[2].Value?.ToString();
                    txtLoaiSP.Text = selectedRow.Cells[3].Value?.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            try
            {
                // Lấy mã sản phẩm từ textbox (được điền từ DataGridView khi người dùng chọn dòng)
                string maSP = txtMaSP.Text.Trim();

                // Kiểm tra xem mã sản phẩm có hợp lệ hay không
                if (string.IsNullOrEmpty(maSP))
                {
                    MessageBox.Show("Vui lòng chọn sản phẩm cần xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Xác nhận hành động xóa
                DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa sản phẩm này?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.No)
                {
                    return; // Hủy bỏ nếu người dùng không xác nhận xóa
                }

                // Khởi tạo context để kết nối với cơ sở dữ liệu
                using (Model1 context = new Model1())
                {
                    // Tìm sản phẩm trong cơ sở dữ liệu theo mã sản phẩm
                    SANPHAM sanPhamToDelete = context.SANPHAMs.FirstOrDefault(sp => sp.MaSP == maSP);

                    if (sanPhamToDelete == null)
                    {
                        MessageBox.Show("Sản phẩm không tồn tại.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // Xóa sản phẩm
                    context.SANPHAMs.Remove(sanPhamToDelete);
                    context.SaveChanges();

                    // Thông báo thành công
                    MessageBox.Show("Xóa sản phẩm thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Cập nhật lại DataGridView
                    List<SANPHAM> listSanPhams = context.SANPHAMs.ToList();
                    BindGrid(listSanPhams);

                    // Xóa nội dung nhập trên giao diện
                    ClearInputFields();
                    btnLuu.Enabled = true;
                    btnKLuu.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn muốn thoát chương trình?", "Xác nhận thoát", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            
            if (result == DialogResult.No)
            {
                e.Cancel = true;
            }
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void btnTim_Click(object sender, EventArgs e)
        {
            try
            {
                string searchKeyword = txtTim.Text.Trim().ToLower(); // Get search keyword and convert it to lowercase

                if (string.IsNullOrWhiteSpace(searchKeyword)) // Check if the search textbox is empty
                {
                    MessageBox.Show("Vui lòng nhập từ khóa tìm kiếm.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (Model1 context = new Model1())
                {
                    // Fetch the filtered list of products based on the search keyword
                    List<SANPHAM> filteredSanPhams = context.SANPHAMs
                        .Where(sp => sp.TenSP.ToLower().Contains(searchKeyword)) // Search based on product name
                        .ToList();

                    // Clear existing rows in the DataGridView
                    dgvQLSP.Rows.Clear();

                    // Add the filtered products to the DataGridView
                    foreach (var sp in filteredSanPhams)
                    {
                        dgvQLSP.Rows.Add(
                            sp.MaSP,
                            sp.TenSP,
                            sp.NgayNhap.HasValue ? sp.NgayNhap.Value.ToString("dd/MM/yyyy") : "N/A",
                            sp.MaLoai
                        );
                    }

                    // Show the "Hủy Tìm" button to allow the user to reset the search
                    btnHuyTim.Visible = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tìm kiếm: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnHuyTim_Click(object sender, EventArgs e)
        {
            try
            {
                using (Model1 context = new Model1())
                {
                    // Fetch all products to reset the DataGridView
                    List<SANPHAM> allSanPhams = context.SANPHAMs.ToList();

                    // Clear existing rows in the DataGridView
                    dgvQLSP.Rows.Clear();

                    // Add all products to the DataGridView
                    foreach (var sp in allSanPhams)
                    {
                        dgvQLSP.Rows.Add(
                            sp.MaSP,
                            sp.TenSP,
                            sp.NgayNhap.HasValue ? sp.NgayNhap.Value.ToString("dd/MM/yyyy") : "N/A",
                            sp.MaLoai
                        );
                    }

                    // Hide the "Hủy Tìm" button since search is reset
                    btnHuyTim.Visible = false;

                    // Clear the search textbox
                    txtTim.Clear();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi hủy tìm kiếm: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
