function mostrarFormulario() {
    const formulario = document.getElementById("form-transaccion");
    if (formulario.style.display === "none" || formulario.style.display === "") {
        formulario.style.display = "block"; // Mostrar el formulario
    } else {
        formulario.style.display = "none"; // Ocultar el formulario
    }
}


document.getElementById("transaccionForm").addEventListener("submit", async function (e) {
    e.preventDefault();
    
    const fromAddress = document.getElementById("fromAddress").value;
    const toAddress = document.getElementById("toAddress").value;
    const amount = parseFloat(document.getElementById("amount").value);  
    const version = parseInt(document.getElementById("version").value);
    const txSize = parseInt(document.getElementById("txSize").value);

    // este es el campo opcional
    const prevBlockHash = document.getElementById("prevBlockHash").value;


    // Validaciones by robotito
    if (!fromAddress || !toAddress || isNaN(amount) || isNaN(version) || isNaN(txSize)) {
        document.getElementById("respuesta").innerHTML = `
            <div class="alert alert-danger">
                Por favor complete todos los campos correctamente.
            </div>
        `;
        return; 
    }

    const objTransaccion = {
        fromAddress: fromAddress,
        toAddress: toAddress,
        amount: amount,
        version: version,
        txSize: txSize
    };

    //Si relleno el campo opcional del otro hash entonces cambia la url 
    let url;
    if (prevBlockHash) {
        url = "https://localhost:7003/api/block/add-transaction-to-block";
    } else {
        url = "https://localhost:7003/api/block/create-transaction";
    }

    if (prevBlockHash) {
        objTransaccion.prevBlockHash = prevBlockHash}
    ;

    try {
        const res = await fetch(url, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(objTransaccion),
        });

        const result = await res.json();
        console.log(result);

        if (res.ok) {
            document.getElementById("respuesta").innerHTML = `
                <div class="alert alert-success">
                    ${result.message}<br>
                    Transacción ID: ${result.transactionId}<br>
                    Hash Bloque: ${result.blockHash}
                </div>
            `;
        } else {
            
            document.getElementById("respuesta").innerHTML = `
                <div class="alert alert-danger">
                    Error: ${result.message || 'Hubo un problema con la solicitud.'}
                </div>
            `;
        }
    } catch (err) {
        document.getElementById("respuesta").innerHTML = `
            <div class="alert alert-danger">
                Error: ${err.message}
            </div>
        `;
    }
});

// Barra lateral
const toggleBtn = document.getElementById('menuToggle');
const sidebarMenu = document.getElementById('sidebarMenu');

toggleBtn?.addEventListener('click', () => {
    sidebarMenu.classList.toggle('show');
});
