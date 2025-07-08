use crate::config;

use std::{
    io::{Read, Write},
    net::{TcpListener, TcpStream},
    thread,
};

// HTTP return codes
const HTTP_200_OK: &[u8] = b"HTTP/1.1 200 OK\r\n";
const HTTP_404_NOTFOUND: &[u8] = b"HTTP/1.1 404 Not Found\r\n";

// MIME types
const TEXT_HTML: &[u8] = b"Content-Type: text/html\r\n";
const TEXT_PLAIN: &[u8] = b"Content-Type: text/plain\r\n";
const TEXT_CSS: &[u8] = b"Content-Type: text/css\r\n";
const TEXT_JS: &[u8] = b"Content-Type: application/javascript\r\n";
const IMAGE_PNG: &[u8] = b"Content-Type: image/png\r\n";

// jigs
const HTTP_KEEP_ALIVE: &[u8] = b"Connection: keep-alive\r\nKeep-Alive: timeout=5, max=1000\r\n";
const HTTP_CACHE: &[u8] = b"Cache-Control: public, max-age=6\r\n"; // 1 hour cache
// const HTTP_NOCACHE: &[u8] = b"Cache-Control: no-cache\r\n";

// static content
const INDEX_HTML: &[u8] = include_bytes!("../static/index.html");
const LOGO_PNG: &[u8] = include_bytes!("../doc/logo.png");
const CSS_CSS: &[u8] = include_bytes!("../static/css.css");
const JS_JS: &[u8] = include_bytes!("../static/js.js");
const JQUERY_MIN_JS: &[u8] = include_bytes!("../static/cdn/jquery.min.js");

fn error_404(client: &mut TcpStream, method: &[u8], url: &[u8]) {
    client.write(&HTTP_404_NOTFOUND).unwrap();
    client.write(&TEXT_PLAIN).unwrap();
    client.write(b"\r\nmethod: ").unwrap();
    client.write(method).unwrap();
    // client.write(b"\r\nurl: ").unwrap();
    client.write(url).unwrap();
    client.flush().unwrap();
}

fn index(client: &mut TcpStream) {
    client.write(&HTTP_200_OK).unwrap();
    client.write(&HTTP_KEEP_ALIVE).unwrap();
    client.write(&HTTP_CACHE).unwrap();
    client.write(&TEXT_HTML).unwrap();
    client.write(b"\r\n").unwrap();
    client.write(&INDEX_HTML).unwrap();
    client.flush().unwrap();
}

fn logo(client: &mut TcpStream) {
    client.write(&HTTP_200_OK).unwrap();
    client.write(&HTTP_KEEP_ALIVE).unwrap();
    client.write(&HTTP_CACHE).unwrap();
    client.write(&IMAGE_PNG).unwrap();
    client.write(b"\r\n").unwrap();
    client.write(&LOGO_PNG).unwrap();
    client.flush().unwrap();
}

fn css(client: &mut TcpStream) {
    client.write(&HTTP_200_OK).unwrap();
    client.write(&HTTP_KEEP_ALIVE).unwrap();
    client.write(&HTTP_CACHE).unwrap();
    client.write(&TEXT_CSS).unwrap();
    client.write(b"\r\n").unwrap();
    client.write(&CSS_CSS).unwrap();
    client.flush().unwrap();
}

fn js(client: &mut TcpStream) {
    client.write(&HTTP_200_OK).unwrap();
    client.write(&HTTP_KEEP_ALIVE).unwrap();
    client.write(&HTTP_CACHE).unwrap();
    client.write(&TEXT_JS).unwrap();
    client.write(b"\r\n").unwrap();
    client.write(&JS_JS).unwrap();
}

fn jquery(client: &mut TcpStream) {
    client.write(&HTTP_200_OK).unwrap();
    client.write(&HTTP_KEEP_ALIVE).unwrap();
    client.write(&HTTP_CACHE).unwrap();
    client.write(&TEXT_JS).unwrap();
    client.write(b"\r\n").unwrap();
    client.write(&JQUERY_MIN_JS).unwrap();
}

fn router(client: &mut TcpStream) {
    let mut buffer = [0; 1024];
    client.read(&mut buffer).unwrap();

    let request = buffer.split(|&x| x == b'\n').next().unwrap();
    let parts: Vec<&[u8]> = request.split(|&x| x == b' ').collect();
    let (method, url) = (parts[0], parts[1]);

    match (method, url) {
        (b"GET", b"/") | (b"GET", b"/index.html") => index(client),
        (b"GET", b"/favicon.ico") | (b"GET", b"/logo.png") => logo(client),
        (b"GET", b"/css.css") => css(client),
        (b"GET", b"/js.js") => js(client),
        (b"GET", b"/jquery.min.js") => jquery(client),
        _ => error_404(client, method, url),
    }
}

pub fn run() {
    let listener = TcpListener::bind(config::BIND).unwrap();
    eprintln!("server @ http://{}:{}", config::IP, config::PORT);
    for client in listener.incoming() {
        match client {
            Ok(mut client) => {
                thread::spawn(move || router(&mut client));
            }
            Err(e) => {
                eprintln!("Error: {}", e);
            }
        }
    }
}
