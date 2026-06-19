export function initToast(toastEl) {
    const toast = bootstrap.Toast.getOrCreateInstance(toastEl);
    toast.show();
}