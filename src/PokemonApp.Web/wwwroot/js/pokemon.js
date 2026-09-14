document.addEventListener('DOMContentLoaded', function () {
    const modalEl = document.getElementById('pokemonDetailModal');
    const modalBody = document.getElementById('pokemonDetailModalBody');
    const modal = new bootstrap.Modal(modalEl);

    const sendEmailModalEl = document.getElementById('sendEmailModal');
    const sendEmailModalBody = document.getElementById('sendEmailModalBody');
    const sendEmailModal = new bootstrap.Modal(sendEmailModalEl);

    window.openSendEmailForm = function (pokemonId) {
    const params = new URLSearchParams(window.location.search);
    if (pokemonId !== null) {
        params.set('pokemonId', pokemonId);
    }

    sendEmailModalBody.innerHTML = '<div class="text-center">Cargando...</div>';
    sendEmailModal.show();

    fetch(`/Pokemon/SendEmailForm?${params.toString()}`)
        .then(function (response) {
            if (!response.ok) {
                throw new Error('Error al cargar el formulario');
            }
            return response.text();
        })
        .then(function (html) {
            sendEmailModalBody.innerHTML = html;
        })
        .catch(function () {
            sendEmailModalBody.innerHTML = '<div class="alert alert-danger">No se pudo cargar el formulario.</div>';
        });
    };


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

