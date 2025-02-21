import {useState} from "react";
import Path from "../../endpoints.jsx";
import {useNavigate} from "react-router-dom";

function LoginPage() {
    const [username, setUsername] = useState("");
    const [password, setPassword] = useState("");
    const [error, setError] = useState(null);
    const navigate = useNavigate();


    const handleUsernameChange = (e) => {
        setUsername(e.target.value);
    };

    const handlePasswordChange = (e) => {
        setPassword(e.target.value);
    };

    const handleSubmit = async (e) => {
        e.preventDefault();

        const loginData = { username, password };

        try {

            //console.log(loginData);
            const response = await fetch(Path["login"], {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify(loginData),
            });

            if (!response.ok) {
                const errorData = await response.json();
                throw new Error(errorData.message || "Login failed");
            }

            const data = await response.json();
            // Save the bearer token in localStorage
            localStorage.setItem("authToken", data.token);
            // Redirect or other logic can be added here
            navigate("/");
        } catch (err) {
            alert(err);

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
                                    data-validation-required-message="Please enter your username"
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
                                    data-validation-required-message="Please enter your password"
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