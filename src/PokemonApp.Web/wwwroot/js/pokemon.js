document.addEventListener('DOMContentLoaded', function () {
    const modalEl = document.getElementById('pokemonDetailModal');
    const modalBody = document.getElementById('pokemonDetailModalBody');
    const modal = new bootstrap.Modal(modalEl);

    document.querySelectorAll('.pokemon-card').forEach(function (card) {
        card.addEventListener('click', function () {
            const id = card.getAttribute('data-pokemon-id');
            modalBody.innerHTML = '<div class="text-center">Cargando...</div>';
            modal.show();

            fetch(`/Pokemon/Detail/${id}`)
                .then(function (response) {
                    if (!response.ok) {
                        throw new Error('Error al cargar el detalle');
                    }
                    return response.text();
                })
                .then(function (html) {
                    modalBody.innerHTML = html;
                })
                .catch(function () {
                    modalBody.innerHTML = '<div class="alert alert-danger">No se pudo cargar el detalle. Intenta de nuevo.</div>';
                });
        });
    });
});