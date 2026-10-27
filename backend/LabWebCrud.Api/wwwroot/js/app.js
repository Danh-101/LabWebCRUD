const state = { products: [], editingId: null, search: "" };
const $ = (id) => document.getElementById(id);
const money = (number) => new Intl.NumberFormat("vi-VN").format(number) + " ₫";
const integer = (number) => new Intl.NumberFormat("vi-VN").format(number);
let toastTimer;

function notify(message, isError = false) {
  const toast = $("toast");
  toast.textContent = message;
  toast.classList.toggle("error", isError);
  toast.classList.remove("hidden");
  clearTimeout(toastTimer);
  toastTimer = setTimeout(() => toast.classList.add("hidden"), 3500);
}

function getErrorMessage(error) {
  if (!error.response) return "Không kết nối được API. Hãy kiểm tra backend và mạng.";
  const data = error.response.data;
  if (typeof data?.message === "string") return data.message;
  if (data?.errors) return Object.values(data.errors).flat().join(" ");
  return `Có lỗi xảy ra (HTTP ${error.response.status}).`;
}

async function loadProducts() {
  try {
    const response = await productApi.getAll();
    state.products = response.data;
    render();
  } catch (error) {
    $("productRows").replaceChildren();
    const row = document.createElement("tr");
    const cell = document.createElement("td");
    cell.colSpan = 6;
    cell.className = "empty-cell";
    cell.textContent = getErrorMessage(error);
    row.append(cell);
    $("productRows").append(row);
    notify(getErrorMessage(error), true);
  }
}

function render() {
  const products = state.products;
  $("totalProducts").textContent = integer(products.length);
  $("totalQuantity").textContent = integer(products.reduce((sum, p) => sum + p.quantity, 0));
  $("inventoryValue").textContent = money(products.reduce((sum, p) => sum + p.price * p.quantity, 0));

  const filtered = products.filter((p) => p.name.toLocaleLowerCase("vi-VN").includes(state.search));
  $("countLabel").textContent = `Hiển thị ${filtered.length} / ${products.length} sản phẩm`;
  const tbody = $("productRows");
  tbody.replaceChildren();

  if (!filtered.length) {
    const tr = document.createElement("tr");
    const td = document.createElement("td");
    td.className = "empty-cell";
    td.colSpan = 6;
    td.textContent = products.length ? "Không tìm thấy sản phẩm phù hợp." : "Chưa có sản phẩm. Hãy bấm Thêm sản phẩm.";
    tr.append(td);
    tbody.append(tr);
    return;
  }

  for (const product of filtered) {
    const row = document.createElement("tr");
    const addCell = (text, className = "") => {
      const cell = document.createElement("td");
      if (className) cell.className = className;
      cell.textContent = text;
      row.append(cell);
      return cell;
    };
    const idCell = document.createElement("td");
    const idSpan = document.createElement("span");
    idSpan.className = "id-pill";
    idSpan.textContent = `#${String(product.id).padStart(3, "0")}`;
    idCell.append(idSpan);
    row.append(idCell);

    const nameCell = document.createElement("td");
    const name = document.createElement("div");
    name.className = "product-name";
    name.textContent = product.name;
    const description = document.createElement("div");
    description.className = "product-description";
    description.textContent = product.description || "Chưa có mô tả";
    nameCell.append(name, description);
    row.append(nameCell);

    addCell(money(product.price), "price-text");
    const stockCell = document.createElement("td");
    const stock = document.createElement("span");
    stock.className = `stock-pill${product.quantity < 5 ? " low" : ""}`;
    stock.textContent = integer(product.quantity);
    stockCell.append(stock);
    row.append(stockCell);
    addCell(money(product.price * product.quantity));

    const actions = document.createElement("td");
    actions.className = "right";
    const editBtn = document.createElement("button");
    editBtn.type = "button";
    editBtn.className = "action-btn";
    editBtn.textContent = "Sửa";
    editBtn.addEventListener("click", () => openEdit(product.id));
    const deleteBtn = document.createElement("button");
    deleteBtn.type = "button";
    deleteBtn.className = "action-btn danger";
    deleteBtn.textContent = "Xóa";
    deleteBtn.addEventListener("click", () => deleteProduct(product));
    actions.append(editBtn, deleteBtn);
    row.append(actions);
    tbody.append(row);
  }
}

function openCreate() {
  state.editingId = null;
  $("productForm").reset();
  $("modalTitle").textContent = "Thêm sản phẩm";
  $("saveButton").textContent = "Thêm sản phẩm";
  $("modalBackdrop").classList.remove("hidden");
  $("name").focus();
}

async function openEdit(id) {
  try {
    // GET chi tiet theo ID la request thu 2 trong bai.
    const { data: product } = await productApi.getById(id);
    state.editingId = id;
    $("name").value = product.name;
    $("description").value = product.description ?? "";
    $("price").value = product.price;
    $("quantity").value = product.quantity;
    $("modalTitle").textContent = `Sửa sản phẩm #${id}`;
    $("saveButton").textContent = "Lưu thay đổi";
    $("modalBackdrop").classList.remove("hidden");
    $("name").focus();
  } catch (error) { notify(getErrorMessage(error), true); }
}

function closeModal() {
  $("modalBackdrop").classList.add("hidden");
  state.editingId = null;
}

async function saveProduct(event) {
  event.preventDefault();
  const data = {
    name: $("name").value.trim(),
    price: Number($("price").value),
    quantity: Number($("quantity").value),
    description: $("description").value.trim()
  };
  if (!data.name) return notify("Vui lòng nhập tên sản phẩm.", true);
  if (!Number.isInteger(data.price) || data.price < 0 || !Number.isInteger(data.quantity) || data.quantity < 0)
    return notify("Giá và số lượng phải là số nguyên không âm.", true);

  $("saveButton").disabled = true;
  try {
    if (state.editingId === null) {
      await productApi.create(data);   // Axios POST
      notify("Đã thêm sản phẩm thành công!");
    } else {
      await productApi.update(state.editingId, data); // Axios PUT
      notify("Đã cập nhật sản phẩm!");
    }
    closeModal();
    await loadProducts();
  } catch (error) {
    notify(getErrorMessage(error), true);
  } finally {
    $("saveButton").disabled = false;
  }
}

async function deleteProduct(product) {
  if (!window.confirm(`Bạn có chắc muốn xóa "${product.name}" (#${product.id})?`)) return;
  try {
    await productApi.remove(product.id); // Axios DELETE
    notify("Đã xóa sản phẩm.");
    await loadProducts();
  } catch (error) { notify(getErrorMessage(error), true); }
}

$("addButton").addEventListener("click", openCreate);
$("refreshButton").addEventListener("click", loadProducts);
$("searchInput").addEventListener("input", (event) => {
  state.search = event.target.value.toLocaleLowerCase("vi-VN").trim();
  render();
});
$("closeButton").addEventListener("click", closeModal);
$("cancelButton").addEventListener("click", closeModal);
$("modalBackdrop").addEventListener("click", (event) => {
  if (event.target === $("modalBackdrop")) closeModal();
});
document.addEventListener("keydown", (event) => {
  if (event.key === "Escape") closeModal();
});
$("productForm").addEventListener("submit", saveProduct);
loadProducts();
