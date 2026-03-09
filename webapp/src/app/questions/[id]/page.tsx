import { getQuestionById } from "@/lib/actions/question-actions";
import QuestionDetailsHeader from "./QuestionDetailsHeader";
import QuestionContent from "./QuestionContent";
import AnswerContent from "./AnswerContent";
import AnswersHeader from "./AnswerHeader";

type Params = Promise<{ id: string }>;

export default async function QuestionDetailed({ params }: Readonly<{ params: Params }>) {
    const { id } = await params;
    const question = await getQuestionById(id);
    console.log(question);
    return (
        <div className="w-full">
            <QuestionDetailsHeader question={question} />
            <QuestionContent question={question} />
            {question.answers.length > 0 && (
                <AnswersHeader answerCount={question.answers.length} />
            )};
            {question.answers.map(answer => (
                <AnswerContent key={answer.id} answer={answer} />
            ))}
        </div>
    )
}