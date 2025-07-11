import React from "react";
import { Container, Navbar, Nav } from "react-bootstrap";
import { Link } from "react-router-dom";

export default function Layout({ children }) {
    return (
        <>
            <Navbar bg="dark" variant="dark" expand="lg" fixed="top">
                <Container>
                    <Navbar.Brand as={Link} to="/">Minha App</Navbar.Brand>
                    <Nav className="me-auto">
                        <Nav.Link as={Link} to="/">Home</Nav.Link>
                        <Nav.Link as={Link} to="/about">Sobre</Nav.Link>
                    </Nav>
                </Container>
            </Navbar>

            <main style={{ marginTop: "70px", minHeight: "calc(100vh - 120px)" }}>
                <Container>{children}</Container>
            </main>

            <footer className="bg-dark text-white text-center py-3 mt-auto">
                © 2025 Minha App. Todos os direitos reservados.
            </footer>
        </>
    );
}


