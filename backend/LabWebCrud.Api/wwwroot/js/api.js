// Tat ca HTTP request tu Frontend den ASP.NET Core backend deu dung Axios.
// Frontend va Backend duoc chay cung domain, vi vay baseURL la duong dan tuong doi.
const apiClient = axios.create({
  baseURL: "/api/products",
  timeout: 10000,
  headers: { "Content-Type": "application/json" }
});

window.productApi = {
  getAll: () => apiClient.get(""),                 // GET /api/products
  getById: (id) => apiClient.get(`/${id}`),        // GET /api/products/{id}
  create: (data) => apiClient.post("", data),      // POST /api/products
  update: (id, data) => apiClient.put(`/${id}`, data), // PUT /api/products/{id}
  remove: (id) => apiClient.delete(`/${id}`)       // DELETE /api/products/{id}
};
