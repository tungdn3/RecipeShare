export default function formatNumber(value: number | undefined) {
  if (value === undefined || value === null) {
    return '';
  }
  return new Intl.NumberFormat().format(value);
}
