const loginForm = document.querySelector('#login-form');
loginForm.addEventListener('submit', (event) => {
    event.preventDefault();
    const email = loginForm.elements.email.value;
    const password = loginForm.elements.password.value;
    // TODO: validate email and password
    login(email, password);
});

const signupForm = document.querySelector('#signup-form');
signupForm.addEventListener('submit', (event) => {
    event.preventDefault();
    const email = signupForm.elements.email.value;
    const password = signupForm.elements.password.value;
    // TODO: validate email and password
    signup(email, password);
});

function login(email, password) {
    // TODO: send login request to server using AJAX
    console.log('Logging in with email:', email, 'and password:', password);
}

function signup(email, password) {
    // TODO: send signup request to server using AJAX
    console.log('Signing up with email:', email, 'and password:', password);
}