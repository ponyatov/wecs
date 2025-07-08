pub const IP: &str = "127.0.0.1";
// pub const IP = "0.0.0.0";
pub const PORT: u16 = 12345;
pub const BIND: &str = const_format::formatcp!("{IP}:{PORT}");
