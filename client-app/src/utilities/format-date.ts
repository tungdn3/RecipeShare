export default function formatDate(value: Date | undefined) {
  if (!value) {
    return '';
  }
  const date = new Date(value);
  const locale = (navigator && navigator.language) || 'en';
  const formatter = new Intl.DateTimeFormat(locale, {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
    second: '2-digit',
  });
  return formatter.format(date);
}
