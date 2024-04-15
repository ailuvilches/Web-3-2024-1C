using Clase2.Entidades;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Windows.Forms;

namespace Clase2.WinForm
{
    public partial class Form1 : Form
    {
        private HttpClient httpClient;

        public Form1()
        {
            InitializeComponent();
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("MyApp", "1.0"));
        }

        private async Task ObtenerResultadosApi()
        {
            List<Resultado> resultados = await GetDataFromApi<List<Resultado>>("https://localhost:7169/api/Resultados");
            if (resultados != null)
            {
                gvResultados.Rows.Clear();
                foreach (var resultado in resultados)
                {
                    AgregarResultadoAGrilla(gvResultados, resultado);
                }
            }
        }

        private async Task ObtenerEquiposApi()
        {
            List<Equipo> equipos = await GetDataFromApi<List<Equipo>>("https://localhost:7169/api/Equipos");
            if (equipos != null)
            {
                gvEquipos.Rows.Clear();
                foreach (var equipo in equipos)
                {
                    AgregarEquipoAGrilla(gvEquipos, equipo);
                }
            }
        }
        
        private async Task AgregarEquiposApi()
        {

            string url = "https://localhost:7169/api/Equipos";

            string jsonBody = $"{{\"nombre_equipo\": \"{txtEquipoACargar.Text}\"," +
                              $"\"pais\": \"{txtPais.Text}\"}}";

            await RealizarSolicitudHttpAsync(url, HttpMethod.Post, jsonBody);
        }

        private async Task AgregarResultadoAApi()
        {
            string url = "https://localhost:7169/api/Resultados";

            string jsonBody = $"{{\"equipoLocal\": \"{txtEquipoLocal.Text}\"," +
                $"\"equipoVisitante\": \"{txtEquipoVisitante.Text}\"," +
                $"\"golesLocal\": \"{cboGolesLocal.Text}\"," +
                $"\"golesVisitante\": \"{cboGolesVisitante.Text}\"}}";

            await RealizarSolicitudHttpAsync(url, HttpMethod.Post, jsonBody);
        }

        private async Task EliminarEquiposApi()
        {
            int equipoId = ObtenerIdEquipoSeleccionado();

            if (equipoId == -1)
            {
                MessageBox.Show("Por favor, seleccione un equipo para eliminar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                string url = $"https://localhost:7169/api/Equipos/id/{equipoId}";

                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.DeleteAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Equipo eliminado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        await ObtenerEquiposApi();
                    }
                    else
                    {
                        string errorResponse = await response.Content.ReadAsStringAsync();

                        MessageBox.Show($"La solicitud falló con el código de estado: {response.StatusCode}\n\n{errorResponse}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al realizar la solicitud: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private async Task ActualizarEquipoApi(Equipo equipo)
        {
            string url = $"https://localhost:7169/api/Equipos";

            string jsonBody = $"{{\"nombre_equipo\": \"{equipo.nombre_equipo}\"," +
                                $"\"id\": \"{equipo.Id}\"," +
                              $"\"pais\": \"{equipo.pais}\"}}";

            await RealizarSolicitudHttpAsync(url, HttpMethod.Put, jsonBody);
        }

        private async Task ActualizarResultadoApi(Resultado resultado)
        {
            string url = $"https://localhost:7169/api/Resultados";

            string jsonBody = $"{{\"id\": \"{resultado.Id}\"," +
                                $"\"equipoLocal\": \"{resultado.EquipoLocal}\"," +
                              $"\"golesLocal\": \"{resultado.GolesLocal}\"," +
                              $"\"golesVisitante\": \"{resultado.GolesVisitante}\"," +
                              $"\"equipoVisitante\": \"{resultado.EquipoVisitante}\"}}";

            await RealizarSolicitudHttpAsync(url, HttpMethod.Put, jsonBody);
        }

        private int ObtenerIdEquipoSeleccionado()
        {
            if (gvEquipos.CurrentRow != null && gvEquipos.CurrentRow.Cells[0].Value != null)
            {
                if (int.TryParse(gvEquipos.CurrentRow.Cells[0].Value.ToString(), out int equipoId))
                {
                    return equipoId;
                }
            }
            return -1;
        }

        //Grillas
        private void AgregarEquipoAGrilla(DataGridView gv, Equipo equipo)
        {
            DataGridViewRow fila = new DataGridViewRow();
            fila.CreateCells(gv);

            fila.Cells[0].Value = equipo.Id;
            fila.Cells[1].Value = equipo.nombre_equipo;
            fila.Cells[2].Value = equipo.pais;

            gv.Rows.Add(fila);
        }

        private void AgregarResultadoAGrilla(DataGridView gv, Resultado res)
        {

            DataGridViewRow fila = new DataGridViewRow();
            fila.CreateCells(gv);

            fila.Cells[0].Value = res.Id;
            fila.Cells[1].Value = res.EquipoLocal;
            fila.Cells[2].Value = res.GolesLocal;
            fila.Cells[3].Value = res.GolesVisitante;
            fila.Cells[4].Value = res.EquipoVisitante;

            gv.Rows.Add(fila);
        }

        //Botones

        private void btnGuardarEquipos_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in gvEquipos.Rows)
            {
                if (row.Cells[0].Value == null)
                    continue;

                string id = row.Cells[0].Value.ToString();
                string nombre = row.Cells[1].Value.ToString();
                string pais = row.Cells[2].Value.ToString();

                Equipo equipo = new Equipo
                {
                    Id = Int32.Parse(id),
                    nombre_equipo = nombre,
                    pais = pais
                };

                ActualizarEquipoApi(equipo);
            }

            ObtenerEquiposApi();
        }

        private void btnGuardarResultados_Click(object sender, EventArgs e)
        {

            foreach (DataGridViewRow row in gvResultados.Rows)
            {
                if (row.Cells[0].Value == null)
                    continue;

                string id = row.Cells[0].Value.ToString();
                string equipoLocal = row.Cells[1].Value.ToString();
                string golLocal = row.Cells[2].Value.ToString();
                string golVisitante = row.Cells[3].Value.ToString();
                string equipoVisitante = row.Cells[4].Value.ToString();

                Resultado resultado = new Resultado
                {
                    Id = Int32.Parse(id),
                    EquipoLocal = equipoLocal,
                    GolesLocal = golLocal,
                    GolesVisitante = golVisitante,
                    EquipoVisitante = equipoVisitante
                };

                ActualizarResultadoApi(resultado);
            }

            ObtenerResultadosApi();
        }

        private void btnCargarResultado_Click(object sender, EventArgs e)
        {
            AgregarResultadoAApi();
        }

        private void btnCargarEquipo_Click(object sender, EventArgs e)
        {
            AgregarEquiposApi();
        }


        private void btnObtenerEquipos_Click(object sender, EventArgs e)
        {
            ObtenerEquiposApi();
        }

        private void btnEliminarEquipo_Click(object sender, EventArgs e)
        {
            EliminarEquiposApi();
        }

        //Métodos privados

        private async Task RealizarSolicitudHttpAsync(string url, HttpMethod metodo, string jsonBody)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Add("User-Agent", "MyApp");

                HttpContent bodyContent = new StringContent(jsonBody, System.Text.Encoding.UTF8, "application/json");

                try
                {
                    HttpResponseMessage response = null;
                    switch (metodo.Method)
                    {
                        case "POST":
                            response = await client.PostAsync(url, bodyContent);
                            break;
                        case "PUT":
                            response = await client.PutAsync(url, bodyContent);
                            break;
                    }

                    if (response != null && response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();

                        Console.WriteLine("Respuesta del servidor:");
                        Console.WriteLine(responseBody);
                        await ObtenerEquiposApi();
                        await ObtenerResultadosApi();
                    }
                    else
                    {
                        Console.WriteLine($"La solicitud falló con el código de estado: {response.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al realizar la solicitud: {ex.Message}");
                }
            }
        }

        private async Task<T> GetDataFromApi<T>(string url)
        {
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<T>(responseBody);
                }
                else
                {
                    Console.WriteLine($"La solicitud falló con el código de estado: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al realizar la solicitud: {ex.Message}");
            }
            return default(T);
        }
    }
}