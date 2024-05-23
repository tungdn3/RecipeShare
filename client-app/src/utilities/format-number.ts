export default function formatNumber(value: number | undefined) {
  if (!value) {
    return '';
  }
  return new Intl.NumberFormat().format(value);
}
