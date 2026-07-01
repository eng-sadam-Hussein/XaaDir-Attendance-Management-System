const variants = {
  primary: "btn-primary",
  orange: "btn-orange",
  ghost: "btn-ghost",
  danger: "btn-danger"
};

export default function Button({ children, variant = "primary", className = "", ...props }) {
  return (
    <button className={`btn ${variants[variant] || variants.primary} ${className}`} {...props}>
      {children}
    </button>
  );
}
