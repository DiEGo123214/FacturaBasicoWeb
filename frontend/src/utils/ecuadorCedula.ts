/**
 * Valida una cédula ecuatoriana usando el algoritmo oficial Módulo 10.
 * Retorna true si es válida, false si no.
 */
export function isValidEcuadorianCedula(value: string): boolean {
  const cedula = value.trim();

  // Debe tener exactamente 10 dígitos
  if (!/^\d{10}$/.test(cedula)) return false;

  // Los 2 primeros dígitos = provincia válida (01 - 24)
  const province = Number(cedula.slice(0, 2));
  if (province < 1 || province > 24) return false;

  // El 3er dígito debe ser menor a 6 (personas naturales)
  const thirdDigit = Number(cedula[2]);
  if (thirdDigit >= 6) return false;

  // Algoritmo Módulo 10
  let total = 0;
  for (let i = 0; i < 9; i++) {
    let digit = Number(cedula[i]);
    if (i % 2 === 0) {       // posición impar (índice par): × 2
      digit *= 2;
      if (digit > 9) digit -= 9;  // si ≥ 10, restar 9
    }
    total += digit;
  }

  const verifier = (10 - (total % 10)) % 10;
  return verifier === Number(cedula[9]);
}

/**
 * Retorna un mensaje de error descriptivo, o null si la cédula es válida.
 * Útil para mostrar mensajes específicos en formularios.
 */
export function validateCedulaMessage(value: string): string | null {
  const cedula = value.trim();

  if (!cedula) return 'La cédula es requerida.';
  if (!/^\d+$/.test(cedula)) return 'La cédula solo debe contener números.';
  if (cedula.length !== 10) return 'La cédula debe tener exactamente 10 dígitos.';

  const province = Number(cedula.slice(0, 2));
  if (province < 1 || province > 24) return 'Los primeros 2 dígitos deben corresponder a una provincia válida (01-24).';

  const thirdDigit = Number(cedula[2]);
  if (thirdDigit >= 6) return 'El tercer dígito debe ser menor a 6.';

  let total = 0;
  for (let i = 0; i < 9; i++) {
    let digit = Number(cedula[i]);
    if (i % 2 === 0) {
      digit *= 2;
      if (digit > 9) digit -= 9;
    }
    total += digit;
  }

  const verifier = (10 - (total % 10)) % 10;
  if (verifier !== Number(cedula[9])) return 'La cédula no es válida (dígito verificador incorrecto).';

  return null; // ✅ válida
}