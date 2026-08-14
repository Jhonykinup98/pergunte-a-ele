export default function FormInput({ label, type = 'text', value, onChange, required = true }) {
  return (
    <div className="field">
      <label>{label}</label>
      <input type={type} value={value} onChange={onChange} required={required} />
    </div>
  )
}
