export function FaqItem({ question, answer }: { question: string; answer: string }) {
  return (
    <details className="group py-4">
      <summary className="cursor-pointer list-none font-medium text-ink marker:content-none">
        <span className="flex items-center justify-between gap-4">
          {question}
          <span aria-hidden className="text-accent group-open:rotate-45">
            +
          </span>
        </span>
      </summary>
      <p className="mt-3 text-sm leading-relaxed text-ink-muted">{answer}</p>
    </details>
  );
}
