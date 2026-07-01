/** @type {import('tailwindcss').Config} */
export default {
  content: ["./index.html", "./src/**/*.{js,jsx}"],
  theme: {
    extend: {
      colors: {
        xaadirBlue: "#0B63CE",
        xaadirBlueLight: "#2186EA",
        xaadirGreen: "#14B84A",
        xaadirOrange: "#F57C00",
        xaadirInk: "#0F172A"
      },
      boxShadow: {
        soft: "0 20px 60px rgba(15, 23, 42, 0.10)",
        glow: "0 16px 36px rgba(11, 99, 206, 0.22)"
      },
      borderRadius: {
        "3xl": "1.6rem",
        "4xl": "2rem"
      }
    }
  },
  plugins: []
};
