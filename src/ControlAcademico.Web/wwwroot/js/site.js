// Control Académico · Fase 2 — comportamiento común de la interfaz.
// Se carga en todas las páginas desde _Layout.cshtml. Cada bloque numerado es independiente:
//   1) toasts  2) modal de eliminar  3) botón Guardar  4) paginación  5) gráfica del dashboard.
(function () {
  'use strict';

  // 1) Avisos tipo toast (los mensajes llegan desde TempData en el layout).
  document.querySelectorAll('.toast[data-auto]').forEach(function (el) {
    if (window.bootstrap) { new bootstrap.Toast(el, { delay: 5000 }).show(); }
    else { el.classList.add('show'); }
  });

  // 2) Modal de confirmación antes de eliminar.
  //    El enlace conserva su href (la página de confirmación) como respaldo si no hay JavaScript.
  document.addEventListener('click', function (e) {
    var enlace = e.target.closest('[data-eliminar]');
    if (!enlace) return;
    var modal = document.getElementById('modalEliminar');
    if (!modal || !window.bootstrap) return;
    e.preventDefault();
    modal.querySelector('form').setAttribute('action', enlace.getAttribute('href'));
    modal.querySelector('[data-nombre]').textContent = enlace.getAttribute('data-eliminar') || 'este registro';
    bootstrap.Modal.getOrCreateInstance(modal).show();
  });

  // 3) Botón Guardar deshabilitado mientras se envía el formulario.
  document.addEventListener('submit', function (e) {
    var form = e.target;
    if (!form.matches('form[data-guardar]')) return;
    // Se espera a que termine la validación del cliente; si la canceló, no se deshabilita.
    setTimeout(function () {
      if (e.defaultPrevented) return;
      form.querySelectorAll('button[type=submit]').forEach(function (b) {
        b.disabled = true;
        b.dataset.texto = b.innerHTML;
        b.innerHTML = '<span class="spinner-border spinner-border-sm me-1" aria-hidden="true"></span>Guardando...';
      });
    }, 0);
  });
  // Si se vuelve con "atrás", se restauran los botones.
  window.addEventListener('pageshow', function () {
    document.querySelectorAll('button[type=submit][data-texto]').forEach(function (b) {
      b.disabled = false; b.innerHTML = b.dataset.texto;
    });
  });

  // 4) Paginación en el navegador para las tablas con data-paginar="N".
  document.querySelectorAll('table[data-paginar]').forEach(function (tabla) {
    var porPagina = parseInt(tabla.getAttribute('data-paginar'), 10) || 10;
    var filas = Array.prototype.slice.call(tabla.querySelectorAll('tbody tr'));
    if (filas.length <= porPagina) return;

    var paginas = Math.ceil(filas.length / porPagina);
    var nav = document.createElement('nav');
    nav.className = 'mt-3';
    nav.setAttribute('aria-label', 'Paginación');
    var ul = document.createElement('ul');
    ul.className = 'pagination pagination-sm justify-content-center flex-wrap mb-0';
    nav.appendChild(ul);
    var contenedor = tabla.closest('.tabla-wrap') || tabla;
    contenedor.parentNode.insertBefore(nav, contenedor.nextSibling);

    function mostrar(pagina) {
      filas.forEach(function (fila, i) {
        fila.style.display = (i >= (pagina - 1) * porPagina && i < pagina * porPagina) ? '' : 'none';
      });
      ul.innerHTML = '';
      for (var p = 1; p <= paginas; p++) {
        var li = document.createElement('li');
        li.className = 'page-item' + (p === pagina ? ' active' : '');
        var a = document.createElement('a');
        a.className = 'page-link'; a.href = '#'; a.textContent = p;
        a.addEventListener('click', (function (n) { return function (ev) { ev.preventDefault(); mostrar(n); }; })(p));
        li.appendChild(a); ul.appendChild(li);
      }
    }
    mostrar(1);
  });

  // 5) Gráfica de inscritos por curso (Chart.js). Si la librería no carga (sin internet), se dibujan barras simples.
  var lienzo = document.getElementById('graficaInscritos');
  if (lienzo) {
    var etiquetas = JSON.parse(lienzo.getAttribute('data-etiquetas') || '[]');
    var valores = JSON.parse(lienzo.getAttribute('data-valores') || '[]');
    if (window.Chart) {
      new Chart(lienzo, {
        type: 'bar',
        data: { labels: etiquetas, datasets: [{ label: 'Inscritos', data: valores, backgroundColor: '#0F766E', borderRadius: 8, maxBarThickness: 48 }] },
        options: {
          responsive: true, maintainAspectRatio: false,
          plugins: { legend: { display: false } },
          scales: { y: { beginAtZero: true, ticks: { precision: 0 } }, x: { grid: { display: false } } }
        }
      });
    } else {
      var mayor = Math.max.apply(null, valores.concat([1]));
      var html = '';
      etiquetas.forEach(function (nombre, i) {
        html += '<div class="mb-3"><div class="d-flex justify-content-between"><span></span><span class="text-muted">' + valores[i] +
          '</span></div><div class="barra-respaldo"><div style="width:' + Math.round(valores[i] * 100 / mayor) + '%"></div></div></div>';
      });
      var cont = document.createElement('div');
      cont.innerHTML = html;
      cont.querySelectorAll('span:first-child').forEach(function (s, i) { s.textContent = etiquetas[i]; });
      lienzo.parentNode.replaceChild(cont, lienzo);
    }
  }
})();