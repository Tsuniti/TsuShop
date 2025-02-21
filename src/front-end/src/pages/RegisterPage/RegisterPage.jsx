import {useState} from "react";
import Path from "../../endpoints.jsx";
import {useNavigate} from "react-router-dom";

function LoginPage() {
    const [username, setUsername] = useState("");
    const [password, setPassword] = useState("");
    const [repeatPassword, setRepeatPassword] = useState("");
    const [error, setError] = useState(null);
    const navigate = useNavigate();

    const handleUsernameChange = (e) => {
        setUsername(e.target.value);
        setError("");
    };

    const handlePasswordChange = (e) => {
        setPassword(e.target.value);
        setError("");

    };

    const handleRepeatPasswordChange = (e) => {
        setRepeatPassword(e.target.value);
        setError("");
    };

    const validateInputs = () => {
        if (username.length < 6) {
            setError("Username must be at least 6 characters long.");
            return false;
        }

        const passwordRegex = /^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$/;
        if (!passwordRegex.test(password)) {
            setError(
                "Password must be at least 8 characters long and contain at least one letter, one number, and one special character (@$!%*?&)."
            );
            return false;
        }

        if (password !== repeatPassword) {
            setError("Passwords do not match.");
            return false;
        }

        setError(null);
        return true;
    };

    const handleSubmit = async (e) => {
        e.preventDefault();

        if (!validateInputs()) {
            return;
        }

        const loginData = { username, password };
        console.log(loginData);

        try {
            const response = await fetch(Path["register"], {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify(loginData),
            });

            if (!response.ok) {
                const errorData = await response.json();
                throw new Error(errorData.message || "Register failed");
            }
            navigate("/login");
        } catch (err) {
            setError(err.message);
        }
    };



    return (
        <div className="container-fluid">
            <h2 className="section-title position-relative text-uppercase mx-xl-5 mb-4"><span
                className="bg-secondary pr-3">Login</span></h2>
            <div className="row px-xl-5 justify-content-center align-items-center">
                <div className="col-lg-7 mb-5">
                    <div className="contact-form bg-light p-30">
                        <div id="success"></div>
                        <form name="login" id="loginForm" onSubmit={handleSubmit}>
                            <div className="control-group">
                                <input
                                    type="text"
                                    className="form-control"
                                    id="username"
                                    placeholder="Username"
                                    value={username}
                                    onChange={handleUsernameChange}
                                    required="required"
                                    data-validation-required-message="Please enter username"
                                />
                                <p className="help-block text-danger"></p>
                            </div>
                            <div className="control-group">
                                <input
                                    type="password"
                                    className="form-control"
                                    id="password"
                                    placeholder="Password"
                                    value={password}
                                    onChange={handlePasswordChange}
                                    required="required"
                                    data-validation-required-message="Please enter password"
                                />
                                <p className="help-block text-danger"></p>
                            </div>
                            <div className="control-group">
                                <input
                                    type="password"
                                    className="form-control"
                                    id="Repeat password"
                                    placeholder="Repeat password"
                                    value={repeatPassword}
                                    onChange={handleRepeatPasswordChange}
                                    required="required"
                                    data-validation-required-message="Repeat password"
                                />
                                <p className="help-block text-danger"></p>
                            </div>
                            {error && <p className="text-danger">{error}</p>}
                            <div>
                                <button className="btn btn-primary py-2 px-4" type="submit" id="sendMessageButton">
                                    Login
                                </button>
                            </div>
                        </form>
                    </div>
                </div>
            </div>
        </div>
    )
}

export default LoginPage;